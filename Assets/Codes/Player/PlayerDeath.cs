using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    private PlayerAnimator playerAnimator;
    private PlayerMoviment playerMoviment;
    private Animator animator;
    private bool isDead = false;

    void Start()
    {
        playerAnimator = GetComponent<PlayerAnimator>();
        playerMoviment = GetComponent<PlayerMoviment>();
        animator = GetComponentInChildren<Animator>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Trap") && !isDead)
        {
            isDead = true;
            StartCoroutine(DeathSequence());
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Trap") && !isDead)
        {
            isDead = true;
            StartCoroutine(DeathSequence());
        }
    }

    private IEnumerator DeathSequence()
    {
        // Desativa movimento
        if (playerMoviment != null)
            playerMoviment.enabled = false;

        // Dispara animação de morte
        if (playerAnimator != null)
            playerAnimator.TriggerDeath();

        if (animator != null)
        {
            // Espera 1 frame para o Animator processar o trigger
            yield return null;

            // Aguarda até o estado "dead" estar tocando
            yield return new WaitUntil(() =>
                animator.GetCurrentAnimatorStateInfo(0).IsName("dead"));

            // Aguarda a animação "dead" terminar completamente
            yield return new WaitUntil(() =>
                animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        }
        else
        {
            yield return new WaitForSeconds(2f);
        }

        SceneManager.LoadScene("Fail");
    }
}