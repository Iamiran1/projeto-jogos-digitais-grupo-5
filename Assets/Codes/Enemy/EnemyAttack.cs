using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Ataque")]
    public float attackRange = 2f;
    public float attackCooldown = 1f;
    public int attackDamage = 10;

    [Header("Refer�ncias")]
    public Transform player;

    [Header("Som")]
    public AudioClip somAtaque;
    [Range(0f, 1f)]
    public float volumeAtaque = 1f;
    public float distanciaMaxSom = 10f;

    private EnemyAnimator enemyAnimator;
    private EnemyMovement enemyMovement;
    private Rigidbody2D rb;
    private AudioSource audioSource;
    private float lastAttackTime;
    private bool isAttacking = false;
    private PlayerDeath playerDeath;   // FIX: cachear p/ checar morte do player

    void Start()
    {
        enemyAnimator = GetComponent<EnemyAnimator>();
        enemyMovement = GetComponent<EnemyMovement>();
        rb = GetComponent<Rigidbody2D>();

        if (player == null)
            player = GameObject.FindWithTag("Player").transform;

        // FIX: cacheamos PlayerDeath para abortar ataques quando o player morrer.
        if (player != null)
            playerDeath = player.GetComponent<PlayerDeath>();

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake = false;
        audioSource.loop = false;

        DisableHitbox();
    }

    void Update()
    {
        // FIX: se o player está morto, não atacamos mais e zeramos a velocidade
        // do inimigo. Evita que o inimigo continue tentando atacar durante a
        // sequência de morte do player.
        if (playerDeath != null && playerDeath.IsDead)
        {
            if (rb != null) rb.linearVelocity = Vector2.zero;
            isAttacking = false;
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            enemyMovement.enabled = false;

            if (Time.time >= lastAttackTime + attackCooldown && !isAttacking)
                Attack();
        }
        else
        {
            if (!isAttacking)
                enemyMovement.enabled = true;
        }
    }

    void Attack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        rb.linearVelocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezePositionX |
                         RigidbodyConstraints2D.FreezeRotation;

        enemyAnimator.TriggerAttack();
        Invoke(nameof(FinishAttack), attackCooldown);
    }

    void FinishAttack()
    {
        isAttacking = false;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        DisableHitbox();
    }

    private float GetVolumeByDistance()
    {
        if (player == null) return 0f;
        float distancia = Vector2.Distance(transform.position, player.position);
        return Mathf.Clamp01(1f - (distancia / distanciaMaxSom)) * volumeAtaque;
    }

    public void EnableHitbox()
    {
        Transform hitbox = transform.Find("AttackHitbox");
        if (hitbox != null)
            hitbox.GetComponent<Collider2D>().enabled = true;

        // Toca o som no momento do ataque via Animation Event
        if (somAtaque != null)
            audioSource.PlayOneShot(somAtaque, GetVolumeByDistance());
    }

    public void DisableHitbox()
    {
        Transform hitbox = transform.Find("AttackHitbox");
        if (hitbox != null)
            hitbox.GetComponent<Collider2D>().enabled = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}