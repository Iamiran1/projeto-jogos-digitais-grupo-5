using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelecaoNiveisController : MonoBehaviour
{
    [SerializeField] public Button[] levelButtons;

    private void Start()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            bool unlocked = GameManager.IsLevelUnlocked(i + 1);
            levelButtons[i].interactable = unlocked;

            Transform padlock = levelButtons[i].transform.Find("Padlock");
            if (padlock != null)
                padlock.gameObject.SetActive(!unlocked);
        }
    }

    /// <summary>Loads the scene for the selected level. Bind each level button's OnClick passing its level number (1-5).</summary>
    public void SelectLevel(int level)
    {
        GameManager.lastLevelPlayed = level;
        string sceneName = level == 1 ? "SampleScene" : "Level" + level;
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>Returns to the main menu. Bind to the Back button.</summary>
    public void OnBackButton()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }
}
