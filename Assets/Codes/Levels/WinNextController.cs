using UnityEngine;
using UnityEngine.SceneManagement;

public class WinNextController : MonoBehaviour
{
    void Start()
    {
        Debug.Log("WinNext — lastLevelPlayed: " + GameManager.lastLevelPlayed);
    }

    public void OnNextLevel()
    {
        Debug.Log("OnNextLevel chamado — lastLevelPlayed: " + GameManager.lastLevelPlayed);
        int next = GameManager.lastLevelPlayed + 1;
        GameManager.lastLevelPlayed = next;
        Debug.Log("Carregando: " + GameManager.GetSceneName(next));
        SceneManager.LoadScene(GameManager.GetSceneName(next));
    }

    public void OnMainMenu()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }
}