using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEnd : MonoBehaviour
{
    [Header("Level Configuration")]
    [SerializeField] public int levelIndex;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.lastLevelPlayed = levelIndex;
            SceneManager.LoadScene("EntreNiveis");
        }
    }
}
