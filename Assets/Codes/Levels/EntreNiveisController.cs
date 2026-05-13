using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EntreNiveisController : MonoBehaviour
{
    [SerializeField] public TMP_Text congratsText;

    private int lastLevel;

    private void Start()
    {
        lastLevel = GameManager.lastLevelPlayed;

        if (congratsText != null)
            congratsText.text = "Congratulations! You completed Level " + lastLevel + "!";

        GameManager.UnlockNextLevel(lastLevel);
    }

    /// <summary>Loads the next level, or the completion screen when all levels are done. Bind to the Next button.</summary>
    public void OnNextLevel()
    {
        if (lastLevel < 5)
        {
            int next = lastLevel + 1;
            GameManager.lastLevelPlayed = next;
            string sceneName = next == 1 ? "SampleScene" : "Level" + next;
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            SceneManager.LoadScene("Finalizacao");
        }
    }

    /// <summary>Returns to the level selection screen. Bind to the Select Level button.</summary>
    public void OnSelectLevel()
    {
        SceneManager.LoadScene("SelecaoNiveis");
    }

    /// <summary>Returns to the main menu. Bind to the Main Menu button.</summary>
    public void OnMainMenu()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }
}
