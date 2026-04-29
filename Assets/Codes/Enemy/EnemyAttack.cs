using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Ataque")]
    public float attackRange = 1f;
    public float attackCooldown = 2f;
    public int attackDamage = 10;

    [Header("Referências")]
    public Transform player;

    private EnemyAnimator enemyAnimator;
    private float lastAttackTime;

    void Start()
    {
        enemyAnimator = GetComponent<EnemyAnimator>();

        if (player == null)
            player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
        }
    }

    void Attack()
    {
        lastAttackTime = Time.time;
        enemyAnimator.TriggerAttack();

        // Causa dano ao player (descomente quando o PlayerHealth existir)
        // PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        // if (playerHealth != null)
        //     playerHealth.TakeDamage(attackDamage);
    }

    // Mostra o range de ataque no editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
