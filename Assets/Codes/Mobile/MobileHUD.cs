using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MobileHUD : MonoBehaviour
{
    public static MobileHUD Instance { get; private set; }
    public static bool IsPaused { get; private set; }

    [SerializeField] private GameObject gameControlsPanel;
    [SerializeField] private GameObject pausePanel;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoSpawn()
    {
        if (Instance != null) return;
        var prefab = Resources.Load<GameObject>("MobileHUD");
        if (prefab == null)
        {
            Debug.LogWarning("MobileHUD: prefab não encontrado em Assets/Resources/MobileHUD.prefab");
            return;
        }
        Instantiate(prefab);
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        if (pausePanel) pausePanel.SetActive(false);

        EnsureEventSystem();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsureEventSystem();
        MobileInput.Reset();
        if (IsPaused) SetPause(false);
        if (gameControlsPanel)
            gameControlsPanel.SetActive(scene.name.StartsWith("Level"));
    }

    public void TogglePause() => SetPause(!IsPaused);
    public void Resume()      => SetPause(false);

    public void RestartLevel()
    {
        SetPause(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMenu()
    {
        SetPause(false);
        SceneManager.LoadScene("MenuPrincipal");
    }

    void EnsureEventSystem()
    {
        if (FindObjectOfType<EventSystem>() != null) return;
        var go = new GameObject("EventSystem");
        go.AddComponent<EventSystem>();
        go.AddComponent<StandaloneInputModule>();
        DontDestroyOnLoad(go);
    }

    void SetPause(bool paused)
    {
        IsPaused = paused;
        Time.timeScale = paused ? 0f : 1f;
        if (pausePanel) pausePanel.SetActive(paused);
        bool isLevel = SceneManager.GetActiveScene().name.StartsWith("Level");
        if (gameControlsPanel && isLevel)
            gameControlsPanel.SetActive(!paused);
    }
}
