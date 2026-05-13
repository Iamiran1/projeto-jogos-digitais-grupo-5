using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    private const string MaxLevelKey = "MaxLevelUnlocked";

    public static int lastLevelPlayed = 1;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        if (PlayerPrefs.GetInt(MaxLevelKey, 1) < 1)
            PlayerPrefs.SetInt(MaxLevelKey, 1);
    }

    public static bool IsLevelUnlocked(int level)
    {
        return level <= PlayerPrefs.GetInt(MaxLevelKey, 1);
    }

    public static void UnlockNextLevel(int currentLevel)
    {
        int max = PlayerPrefs.GetInt(MaxLevelKey, 1);
        if (currentLevel + 1 > max)
        {
            PlayerPrefs.SetInt(MaxLevelKey, currentLevel + 1);
            PlayerPrefs.Save();
        }
    }

    public static string GetSceneName(int level)
    {
        // Mapeia número do nível para nome da cena
        switch (level)
        {
            case 1: return "Level1";
            case 2: return "Level2";
            case 3: return "Level3";
            case 4: return "Level4";
            case 5: return "Level5";
            case 6: return "Level6";
            case 7: return "Level7";
            case 8: return "Level8";
            case 9: return "Level9";
            case 10: return "Level10";
            default: return "Level1";
        }
    }

    public static void ResetProgress()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
