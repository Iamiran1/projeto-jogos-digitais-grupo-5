using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Patrulha")]
    public float patrolSpeed = 2f;
    public float patrolDistance = 3f;

    [Header("Perseguição")]
    public float chaseSpeed = 4f;
    public float detectionRange = 5f;

    [Header("Referências")]
    public Transform player;

    private Rigidbody2D rb;
    private EnemyAnimator enemyAnimator;
    private Vector2 startPosition;
    private bool movingRight = true;
    private bool isChasing = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyAnimator = GetComponent<EnemyAnimator>();
        startPosition = transform.position;

        // Busca o player automaticamente se não foi assignado
        if (player == null)
            player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
            isChasing = true;
        else
            isChasing = false;

        if (isChasing)
            ChasePlayer();
        else
            Patrol();
    }

    void Patrol()
    {
        enemyAnimator.SetMoving(true);

        float speed = movingRight ? patrolSpeed : -patrolSpeed;
        rb.linearVelocity = new Vector2(speed, rb.linearVelocity.y);

        // Vira o inimigo
        transform.localScale = new Vector3(movingRight ? 1 : -1, 1, 1);

        // Verifica se chegou no limite da patrulha
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

        // Vira o inimigo em direção ao player
        transform.localScale = new Vector3(direction, 1, 1);
    }
}
