using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Ataque")]
    public float attackRange = 2f;
    public float attackCooldown = 1f;
    public int attackDamage = 10;

    [Header("Referências")]
    public Transform player;

    private EnemyAnimator enemyAnimator;
    private EnemyMovement enemyMovement;
    private Rigidbody2D rb;
    private float lastAttackTime;
    private bool isAttacking = false;

    void Start()
    {
        enemyAnimator = GetComponent<EnemyAnimator>();
        enemyMovement = GetComponent<EnemyMovement>();
        rb = GetComponent<Rigidbody2D>();

        if (player == null)
            player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            // Para o movimento sempre que estiver no range
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            enemyMovement.enabled = false;

            // Ataca se o cooldown passou
            if (Time.time >= lastAttackTime + attackCooldown && !isAttacking)
                Attack();
        }
        else
        {
            // Fora do range, reativa o movimento
            if (!isAttacking)
                enemyMovement.enabled = true;
        }
    }

    void Attack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        // Trava completamente o inimigo
        rb.linearVelocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezePositionX |
                         RigidbodyConstraints2D.FreezeRotation;

        enemyAnimator.TriggerAttack();
        Invoke(nameof(FinishAttack), attackCooldown);
    }

    void FinishAttack()
    {
        isAttacking = false;

        // Destrava o movimento
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
