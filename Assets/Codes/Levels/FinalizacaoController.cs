using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalizacaoController : MonoBehaviour
{
    /// <summary>Goes to the level selection screen. Bind to the Select Level button.</summary>
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
