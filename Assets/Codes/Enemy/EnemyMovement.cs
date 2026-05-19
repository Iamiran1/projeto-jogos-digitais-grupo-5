using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Patrulha")]
    public float patrolSpeed = 2f;
    public float patrolDistance = 3f;

    [Header("Persegui��o")]
    public float chaseSpeed = 4f;
    public float detectionRange = 5f;

    [Header("Refer�ncias")]
    public Transform player;

    [Header("Som")]
    public AudioClip somPatrulha;
    [Range(0f, 1f)]
    public float volumePatrulha = 0.7f;
    public float distanciaMaxSom = 10f;

    private Rigidbody2D rb;
    private EnemyAnimator enemyAnimator;
    private AudioSource audioSource;
    private Vector2 startPosition;
    private bool movingRight = true;
    private Vector3 originalScale;
    private PlayerDeath playerDeath;   // FIX: cachear PlayerDeath p/ checar IsDead

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyAnimator = GetComponent<EnemyAnimator>();
        startPosition = transform.position;
        originalScale = transform.localScale;

        if (player == null)
            player = GameObject.FindWithTag("Player").transform;

        // FIX: cachear PlayerDeath para checar se o player morreu — evita
        // que o inimigo continue perseguindo o corpo durante a animação
        // de morte e gere interferência visual.
        if (player != null)
            playerDeath = player.GetComponent<PlayerDeath>();

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = somPatrulha;
        audioSource.loop = true;
        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake = false;
        audioSource.volume = 0f;

        if (somPatrulha != null)
            audioSource.Play();
    }

    void Update()
    {
        // FIX: se o player está morto, paramos o inimigo no lugar. Isso
        // impede que o inimigo continue se movendo em direção ao corpo
        // morto e cause qualquer interferência visual durante a morte.
        if (playerDeath != null && playerDeath.IsDead)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            if (enemyAnimator != null)
                enemyAnimator.SetMoving(false);
            if (audioSource != null)
                audioSource.volume = 0f;
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
            ChasePlayer();
        else
            Patrol();

        // Controla volume por dist�ncia
        if (player != null && audioSource != null)
        {
            float distancia = Vector2.Distance(transform.position, player.position);
            float volumeCalculado = Mathf.Clamp01(1f - (distancia / distanciaMaxSom));
            audioSource.volume = volumeCalculado * volumePatrulha;
        }
    }

    void Patrol()
    {
        enemyAnimator.SetMoving(true);

        float speed = movingRight ? patrolSpeed : -patrolSpeed;
        rb.linearVelocity = new Vector2(speed, rb.linearVelocity.y);

        transform.localScale = new Vector3(
            movingRight ? originalScale.x : -originalScale.x,
            originalScale.y,
            originalScale.z
        );

        if (movingRight && transform.position.x >= startPosition.x + patrolDistance)
            movingRight = false;
        else if (!movingRight && transform.position.x <= startPosition.x - patrolDistance)
            movingRight = true;
    }

    void ChasePlayer()
    {
        enemyAnimator.SetMoving(true);

        float direction = player.position.x > transform.position.x ? 1 : -1;
        rb.linearVelocity = new Vector2(direction * chaseSpeed, rb.linearVelocity.y);

        transform.localScale = new Vector3(
            direction * originalScale.x,
            originalScale.y,
            originalScale.z
        );
    }
}