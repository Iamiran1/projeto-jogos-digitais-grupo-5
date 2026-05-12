using UnityEngine;
using System.Collections;

public class TrapSandworm : MonoBehaviour
{
    private BoxCollider2D damageCollider;
    private Animator anim;
    private bool playerInRange = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        damageCollider = GetComponent<BoxCollider2D>();
        damageCollider.enabled = false;
        anim.speed = 0;
    }

    public void PlayerEnteredRange()
    {
        playerInRange = true;
        anim.speed = 1;
    }

    public void PlayerExitedRange()
    {
        playerInRange = false;
        StartCoroutine(WaitAndStop());
    }

    private IEnumerator WaitAndStop()
    {
        if (!gameObject.activeInHierarchy) yield break;

        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
        float remainingTime = (1f - state.normalizedTime % 1f) * state.length;
        yield return new WaitForSeconds(remainingTime);

        if (!playerInRange && gameObject.activeInHierarchy)
        {
            anim.speed = 0;
            damageCollider.enabled = false;
        }
    }

    public void EnableCollider()
    {
        damageCollider.enabled = true;
    }

    public void DisableCollider()
    {
        damageCollider.enabled = false;
    }
}