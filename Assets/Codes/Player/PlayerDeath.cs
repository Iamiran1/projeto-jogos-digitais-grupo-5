using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    private PlayerAnimator playerAnimator;
    private PlayerMoviment playerMoviment;
    private Animator animator;
    private bool isDead = false;

    [Header("Som")]
    public AudioClip somMorte;
    [Range(0f, 1f)]
    public float volume = 1f;

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
        if (playerMoviment != null)
            playerMoviment.enabled = false;

        if (playerAnimator != null)
            playerAnimator.TriggerDeath();

        // Toca som de morte em objeto persistente
        if (somMorte != null)
        {
            GameObject somTemporario = new GameObject("SomMorte");
            AudioSource somSource = somTemporario.AddComponent<AudioSource>();
            somSource.clip = somMorte;
            somSource.volume = volume;
            DontDestroyOnLoad(somTemporario);
            somSource.Play();
            Destroy(somTemporario, somMorte.length);
        }

        if (animator != null)
        {
            yield return null;

            yield return new WaitUntil(() =>
                animator.GetCurrentAnimatorStateInfo(0).IsName("dead"));

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