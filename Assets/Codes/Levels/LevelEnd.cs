using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelEnd : MonoBehaviour
{
    [Header("Número deste nível (1 a 10)")]
    public int levelIndex;

    [Header("Som")]
    public AudioClip somBandeira;
    [Range(0f, 1f)]
    public float volume = 1f;

    private const int TOTAL_LEVELS = 10;
    private bool triggered = false;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !triggered)
        {
            triggered = true;
            GameManager.lastLevelPlayed = levelIndex;
            GameManager.UnlockNextLevel(levelIndex);

            // Desativa movimento do player
            PlayerMoviment playerMoviment = other.GetComponent<PlayerMoviment>();
            if (playerMoviment != null)
                playerMoviment.enabled = false;

            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic;
            }

            StartCoroutine(FinalizarFase());
        }
    }

    private IEnumerator FinalizarFase()
    {
        if (somBandeira != null && audioSource != null)
        {
            // Cria um objeto temporário para tocar o som e sobreviver à troca de cena
            GameObject somTemporario = new GameObject("SomBandeira");
            AudioSource somSource = somTemporario.AddComponent<AudioSource>();
            somSource.clip = somBandeira;
            somSource.volume = volume;
            DontDestroyOnLoad(somTemporario);
            somSource.Play();

            // Destrói o objeto após o som terminar
            Destroy(somTemporario, somBandeira.length);
        }

        yield return new WaitForSeconds(0.1f);

        if (levelIndex >= TOTAL_LEVELS)
            SceneManager.LoadScene("Finalizacao");
        else
            SceneManager.LoadScene("WinNext");
    }
}