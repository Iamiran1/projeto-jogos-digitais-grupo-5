using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    private const string MaxLevelKey = "MaxLevelUnlocked";

    /// <summary>Index of the last level the player entered or completed.</summary>
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

        // Guarantee level 1 is always unlocked.
        if (PlayerPrefs.GetInt(MaxLevelKey, 1) < 1)
            PlayerPrefs.SetInt(MaxLevelKey, 1);
    }

    /// <summary>Returns true if the given level number is unlocked.</summary>
    public static bool IsLevelUnlocked(int level)
    {
        int max = PlayerPrefs.GetInt(MaxLevelKey, 1);
        return level <= max;
    }

    /// <summary>Unlocks the level after currentLevel if not already unlocked.</summary>
    public static void UnlockNextLevel(int currentLevel)
    {
        int max = PlayerPrefs.GetInt(MaxLevelKey, 1);
        if (currentLevel + 1 > max)
        {
            PlayerPrefs.SetInt(MaxLevelKey, currentLevel + 1);
            PlayerPrefs.Save();
        }
    }

    /// <summary>Deletes all PlayerPrefs — use in debug/testing only.</summary>
    public static void ResetProgress()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt(MaxLevelKey, 1);
        PlayerPrefs.Save();
        Debug.Log("[GameManager] Progress reset.");
    }
}
