using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelecaoNiveisController : MonoBehaviour
{
    [Header("Imagens de fundo - índice 0 = nivel0, índice 1 = nivel1, etc.")]
    [SerializeField] private Sprite[] backgroundSprites; // 11 sprites: nivel0 a nivel10
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Button[] levelButtons; // 10 botões, índice 0 = Level1

    private void Start()
    {
        int maxUnlocked = PlayerPrefs.GetInt("MaxLevelUnlocked", 1);

        // Atualiza imagem de fundo conforme progresso
        // nivel0 = só level1 desbloqueado, nivel1 = level2 desbloqueado, etc.
        int spriteIndex = maxUnlocked - 1; // nivel0 quando max=1, nivel1 quando max=2...
        spriteIndex = Mathf.Clamp(spriteIndex, 0, backgroundSprites.Length - 1);
        if (backgroundImage != null && backgroundSprites.Length > 0)
            backgroundImage.sprite = backgroundSprites[spriteIndex];

        // Ativa/desativa botões conforme desbloqueio
        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (levelButtons[i] != null)
                levelButtons[i].interactable = GameManager.IsLevelUnlocked(i + 1);
        }
    }

    public void SelectLevel(int level)
    {
        GameManager.lastLevelPlayed = level;
        SceneManager.LoadScene(GameManager.GetSceneName(level));
    }

    public void OnBackButton()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }
}
