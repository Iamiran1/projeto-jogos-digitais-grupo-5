using UnityEngine;
using System.Collections;

public class PlayerDeath : MonoBehaviour
{
    private PlayerAnimator playerAnimator;
    private PlayerMoviment playerMoviment;
    private bool isDead = false;

    void Start()
    {
        playerAnimator = GetComponent<PlayerAnimator>();
        playerMoviment = GetComponent<PlayerMoviment>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Trap") && !isDead)
        {
            isDead = true;
            StartCoroutine(DeathSequence());
        }
    }

    private IEnumerator DeathSequence()
    {
        // Para o movimento do player
        if (playerMoviment != null)
            playerMoviment.enabled = false;

        // Toca animação de morte e desativa o animator script
        if (playerAnimator != null)
        {
            playerAnimator.TriggerDeath();
            playerAnimator.enabled = false;
        }

        yield return new WaitForSeconds(2f);

        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}