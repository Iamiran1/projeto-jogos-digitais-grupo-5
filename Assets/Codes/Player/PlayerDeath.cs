using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    private PlayerAnimator playerAnimator;
    private PlayerMoviment playerMoviment;
    private Animator animator;
    private bool isDead = false;

    public bool IsDead => isDead;

    [Header("Som")]
    public AudioClip somMorte;
    [Range(0f, 1f)]
    public float volume = 1f;

    void Start()
    {
        playerAnimator = GetComponent<PlayerAnimator>();
        playerMoviment = GetComponent<PlayerMoviment>();
        animator = GetComponentInChildren<Animator>();

        // Problema 2 — salva o nível atual ao iniciar a cena
        SaveCurrentLevel();
    }

    private void SaveCurrentLevel()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        switch (sceneName)
        {
            case "Level1": GameManager.lastLevelPlayed = 1; break;
            case "Level2": GameManager.lastLevelPlayed = 2; break;
            case "Level3": GameManager.lastLevelPlayed = 3; break;
            case "Level4": GameManager.lastLevelPlayed = 4; break;
            case "Level5": GameManager.lastLevelPlayed = 5; break;
            case "Level6": GameManager.lastLevelPlayed = 6; break;
            case "Level7": GameManager.lastLevelPlayed = 7; break;
            case "Level8": GameManager.lastLevelPlayed = 8; break;
            case "Level9": GameManager.lastLevelPlayed = 9; break;
            case "Level10": GameManager.lastLevelPlayed = 10; break;
        }
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

    // Problema 1 — método público para KillZone chamar
    public void TriggerDeath()
    {
        if (!isDead)
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