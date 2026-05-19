using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    private PlayerAnimator playerAnimator;
    private PlayerMoviment playerMoviment;
    private PlayerPush playerPush;
    private Rigidbody2D rb;
    private Collider2D[] allColliders;
    private AlignVisualToColliderBottom visualAlign;   // FIX: cachear p/ forçar alinhamento

    private bool isDead = false;
    private bool isFrozen = false;        // FIX: distingue "morto e parado" de "morto e caindo"
    private Vector3 deathPosition;        // posição final travada após a morte
    private Vector3 prePhysicsPosition;   // FIX: posição capturada ANTES do physics step
    private Vector3 lastSafePosition;     // FIX: última posição segura (mais conservadora)

    public bool IsDead => isDead;

    [Header("Som")]
    public AudioClip somMorte;
    [Range(0f, 1f)]
    public float volume = 1f;

    [Header("Tempo de Morte")]
    [Tooltip("Quantos segundos esperar entre o player ser acertado e a cena Fail carregar.")]
    public float deathDelay = 2f;

    void Start()
    {
        // FIX: cacheamos TUDO em Start para não fazer GetComponent
        // durante a morte (que precisa ser instantânea, sem custo).
        playerAnimator = GetComponent<PlayerAnimator>();
        playerMoviment = GetComponent<PlayerMoviment>();
        playerPush = GetComponent<PlayerPush>();
        rb = GetComponent<Rigidbody2D>();
        allColliders = GetComponents<Collider2D>();
        visualAlign = GetComponent<AlignVisualToColliderBottom>();   // FIX

        // FIX: inicializa snapshots de posição com a posição inicial do player.
        // Garante que mesmo que o player morra antes do primeiro FixedUpdate,
        // temos uma posição válida (não Vector3.zero, que poderia teleportar
        // o player para a origem do mundo).
        prePhysicsPosition = transform.position;
        lastSafePosition = transform.position;

        SaveCurrentLevel();
    }

    private void SaveCurrentLevel()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        switch (sceneName)
        {
            case "Level1": GameManager.lastLevelPlayed = 1; break;
            case "Level2": GameManager.lastLevelPlayed = 2; break;
            case "Level3": GameManager.lastLevelPlayed = 3; break;
            case "Level4": GameManager.lastLevelPlayed = 4; break;
            case "Level5": GameManager.lastLevelPlayed = 5; break;
            case "Level6": GameManager.lastLevelPlayed = 6; break;
            case "Level7": GameManager.lastLevelPlayed = 7; break;
            case "Level8": GameManager.lastLevelPlayed = 8; break;
            case "Level9": GameManager.lastLevelPlayed = 9; break;
            case "Level10": GameManager.lastLevelPlayed = 10; break;
        }
    }

    void Update()
    {
        // FIX: quando morto, nada de processar input (exceto R desativado abaixo);
        // o Update fica inerte para evitar qualquer interferência.
        if (isDead) return;

        if (Input.GetKeyDown(KeyCode.R))
        if (MobileHUD.IsPaused) return;

        bool restart = Input.GetKeyDown(KeyCode.R) || MobileInput.RestartPressed;
        if (restart)
        {
            MobileInput.RestartPressed = false;
            RestartLevel();
        }
    }

    // FIX CRÍTICO (causa raiz do "player no ar"):
    // O OnTriggerEnter2D dispara DENTRO do physics step, DEPOIS que o solver
    // do Box2D já moveu o player e aplicou o impulso de separação. Por isso
    // a posição em transform.position no instante da morte JÁ ESTÁ ELEVADA.
    //
    // Solução: FixedUpdate roda ANTES do physics step. transform.position aqui
    // reflete a posição do FIM do step anterior — ou seja, ANTES de qualquer
    // impulso aplicado neste step. Esse é o "ponto seguro" a usar como deathPosition.
    void FixedUpdate()
    {
        if (!isDead)
        {
            // Guardamos a posição de DOIS frames atrás como segurança extra.
            // Mesmo se o player começou a sobrepor o trap no frame anterior
            // (mas o trigger ainda não tinha disparado), temos um fallback.
            lastSafePosition = prePhysicsPosition;
            prePhysicsPosition = transform.position;
        }
    }

    // FIX: LateUpdate trava a posição depois que TODA a física e Update já rodaram.
    // Isso garante que mesmo que o solver do Box2D aplique um impulso de
    // separação ao resolver sobreposição de colliders no frame da morte,
    // o player é forçado de volta para a posição capturada no instante da morte.
    void LateUpdate()
    {
        // FIX: só travamos a posição se já estamos congelados (morte no chão
        // ou pouso após morte no ar). Enquanto isFrozen=false, o player está
        // caindo pela gravidade e NÃO devemos travar a posição.
        if (isDead && isFrozen)
        {
            transform.position = deathPosition;

            // segurança extra: se o RB ainda estiver simulado por algum motivo,
            // zeramos qualquer velocidade que possa ter sido aplicada.
            if (rb != null && rb.simulated)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }

            // FIX: força realinhamento do sprite DEPOIS de travar a posição.
            // A ordem entre LateUpdates de scripts diferentes é não-determinística
            // no Unity. Chamando ForceAlign aqui, garantimos que o sprite SEMPRE
            // termina alinhado com deathPosition.
            if (visualAlign != null) visualAlign.ForceAlign();
        }
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Trap") && !isDead)
            Die();
    }

    // FIX: OnTriggerStay2D como rede de segurança. Em colisões muito rápidas
    // (ex: dash atravessando trap em 1-2 frames) o Enter pode disparar,
    // mas se algum collider só sobrepuser por 1 frame durante o dash, o Stay
    // garante a captura. O guard !isDead impede chamada dupla.
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Trap") && !isDead)
            Die();
    }

    public void TriggerDeath()
    {
        if (!isDead)
            Die();
    }

    // FIX: método único centralizado para a morte.
    // Agora trata DOIS casos distintos:
    //   - GROUNDED: player está no chão → teleporta para posição segura
    //     e congela imediatamente (comportamento original).
    //   - IN-AIR: player está no ar (pulando/caindo/dash) → mantém física
    //     e gravidade ATIVAS, deixa o player cair naturalmente. Quando ele
    //     pousar (detectado via OnCollisionEnter2D), congela no chão.
    private void Die()
    {
        if (isDead) return;
        isDead = true;

        bool wasGrounded = playerMoviment != null && playerMoviment.IsGrounded;

        // Sempre desabilita controles e zera componente horizontal de velocidade
        if (playerMoviment != null)
            playerMoviment.enabled = false;

        if (playerPush != null)
            playerPush.enabled = false;

        if (wasGrounded)
        {
            // ===== MORTE NO CHÃO =====
            // 1) Usa snapshot pré-física como posição segura (evita o impulso
            //    para cima que o Box2D aplicou no physics step deste frame).
            deathPosition = prePhysicsPosition;

            // 2) Teleporta IMEDIATAMENTE de volta à posição segura.
            transform.position = deathPosition;
            if (rb != null) rb.position = deathPosition;

            // 3) Congela TUDO: velocidade, gravidade, simulação.
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.gravityScale = 0f;
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.simulated = false;
            }

            isFrozen = true;
        }
        else
        {
            // ===== MORTE NO AR =====
            // NÃO teleporta. NÃO congela física. Deixa a gravidade puxar
            // o player até o chão. O OnCollisionEnter2D vai detectar o
            // pouso e congelar naquele momento.
            //
            // Mantém só a velocidade vertical (queda); zera a horizontal
            // para o player cair reto, sem deslizar lateralmente.
            if (rb != null)
            {
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                rb.angularVelocity = 0f;
            }

            // deathPosition será atualizada quando o player pousar.
            deathPosition = transform.position;
            isFrozen = false;
        }

        // Sincroniza o PlayerAnimator (anima "die" independente de estar caindo).
        if (playerAnimator != null)
            playerAnimator.TriggerDeath();

        // Só faz sentido forçar alinhamento agora se já está congelado;
        // se está caindo, o LateUpdate normal do AlignVisualToColliderBottom
        // continua acompanhando o player frame a frame.
        if (isFrozen && visualAlign != null) visualAlign.ForceAlign();

        // Inicia a sequência assíncrona (som + timeout + carregar Fail).
        StartCoroutine(DeathSequence());
    }

    // FIX: detecta pouso quando o player morre no ar.
    // Quando o CapsuleCollider2D (não-trigger) colide com qualquer
    // superfície que aponte para cima (chão, plataforma, topo de inimigo),
    // congelamos o player no lugar do pouso.
    void OnCollisionEnter2D(Collision2D col)
    {
        if (!isDead || isFrozen) return;

        // Verifica se alguma normal de contato aponta para cima — significa
        // que pousamos em cima de algo (não bateu lateralmente em parede).
        for (int i = 0; i < col.contactCount; i++)
        {
            if (col.GetContact(i).normal.y > 0.5f)
            {
                FreezeAfterLanding();
                return;
            }
        }
    }

    // FIX: congela o player após o pouso de uma morte no ar.
    private void FreezeAfterLanding()
    {
        deathPosition = transform.position;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.gravityScale = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = false;
        }

        isFrozen = true;

        if (visualAlign != null) visualAlign.ForceAlign();
    }

    private IEnumerator DeathSequence()
    {
        if (somMorte != null)
        {
            GameObject somTemporario = new GameObject("SomMorte");
            AudioSource somSource = somTemporario.AddComponent<AudioSource>();
            somSource.clip = somMorte;
            somSource.volume = volume;
            DontDestroyOnLoad(somTemporario);
            somSource.Play();
            Destroy(somTemporario, somMorte.length);
        }

        // FIX: timeout fixo em vez de esperar o estado da animação.
        // Não importa como o player morre nem o estado do Animator —
        // depois de deathDelay segundos, a cena Fail é carregada.
        // Isso elimina qualquer dependência do AnimatorStateInfo,
        // que poderia travar a sequência caso o estado "dead" nunca
        // fosse atingido (ex: transição bloqueada por bool residual).
        yield return new WaitForSeconds(deathDelay);

        SceneManager.LoadScene("Fail");
    }
}
