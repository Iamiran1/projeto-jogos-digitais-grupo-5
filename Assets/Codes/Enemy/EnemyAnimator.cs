using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void SetMoving(bool isMoving)
    {
        anim.SetBool("isMoving", isMoving);
    }

    public void TriggerAttack()
    {
        anim.SetTrigger("Attack");
    }

    public void TriggerDeath()
    {
        anim.SetTrigger("Death");
    }
}
