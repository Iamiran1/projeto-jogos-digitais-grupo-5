using UnityEngine;
using UnityEngine.SceneManagement;

public class WinNextController : MonoBehaviour
{
    public void OnNextLevel()
    {
        int next = GameManager.lastLevelPlayed + 1;
        GameManager.lastLevelPlayed = next;
        SceneManager.LoadScene(GameManager.GetSceneName(next));
    }

    public void OnMainMenu()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }
}
