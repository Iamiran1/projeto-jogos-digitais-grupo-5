using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;

    private bool isPaused;

    private void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    /// <summary>Freezes the game and shows the pause panel. Bind to the Pause button.</summary>
    public void Pause()
    {
        Time.timeScale = 0f;
        isPaused = true;
        if (pausePanel != null)
            pausePanel.SetActive(true);
    }

    /// <summary>Resumes the game and hides the pause panel. Bind to the Resume button.</summary>
    public void Resume()
    {
        Time.timeScale = 1f;
        isPaused = false;
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    /// <summary>Restores time scale and returns to the main menu. Bind to the Main Menu button inside the pause panel.</summary>
    public void BackToMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene("MenuPrincipal");
    }
}
