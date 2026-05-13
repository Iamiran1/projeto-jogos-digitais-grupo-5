using UnityEngine;
using UnityEngine.SceneManagement;

public class FailController : MonoBehaviour
{
    public void OnRestart()
    {
        SceneManager.LoadScene(GameManager.GetSceneName(GameManager.lastLevelPlayed));
    }

    public void OnMainMenu()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }
}
