using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipalController : MonoBehaviour
{
    [SerializeField] private GameObject creditsPanel;

    private void Start()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(false);
    }

    /// <summary>Loads the level selection scene. Bind to the Play button.</summary>
    public void OnPlayButton()
    {
        SceneManager.LoadScene("SelecaoNiveis");
    }

    /// <summary>Shows the credits panel. Bind to the Credits button.</summary>
    public void OnCreditsButton()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(true);
    }

    /// <summary>Hides the credits panel. Bind to the close button inside the panel.</summary>
    public void OnCloseCredits()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(false);
    }
}
