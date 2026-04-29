using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    [Header("Configurações")]
    public float timeToDestroy = 2f;

    private EnemyAnimator enemyAnimator;
    private EnemyMovement enemyMovement;
    private EnemyAttack enemyAttack;
    private bool isDead = false;

    void Start()
    {
        enemyAnimator = GetComponent<EnemyAnimator>();
        enemyMovement = GetComponent<EnemyMovement>();
        enemyAttack = GetComponent<EnemyAttack>();
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;

        // Toca animação de morte
        enemyAnimator.TriggerDeath();

        // Desativa movimento e ataque
        enemyMovement.enabled = false;
        enemyAttack.enabled = false;

        // Desativa o collider
        GetComponent<Collider2D>().enabled = false;

        // Destroi o objeto após a animação terminar
        Destroy(gameObject, timeToDestroy);
    }
}
