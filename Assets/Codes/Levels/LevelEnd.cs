using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEnd : MonoBehaviour
{
    [Header("Número deste nível (1 a 10)")]
    public int levelIndex;

    private const int TOTAL_LEVELS = 10;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.lastLevelPlayed = levelIndex;
            GameManager.UnlockNextLevel(levelIndex);

            if (levelIndex >= TOTAL_LEVELS)
                SceneManager.LoadScene("Finalizacao");
            else
                SceneManager.LoadScene("WinNext");
        }
    }
}
