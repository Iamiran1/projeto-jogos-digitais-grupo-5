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

        DisableHitbox();
    }

    void Update()
    {
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

    public void EnableHitbox()
    {
        Transform hitbox = transform.Find("AttackHitbox");
        if (hitbox != null)
            hitbox.GetComponent<Collider2D>().enabled = true;
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