using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{
    [SerializeField] public TMP_Text levelText;
    [SerializeField] public GameObject[] lifeIcons;

    private void Start()
    {
        if (levelText != null)
            levelText.text = "Level " + GameManager.lastLevelPlayed;
    }

    /// <summary>Shows or hides life icons based on currentLife. Call this from the player health system.</summary>
    public void UpdateLife(int currentLife)
    {
        for (int i = 0; i < lifeIcons.Length; i++)
            lifeIcons[i].SetActive(i < currentLife);
    }
}
