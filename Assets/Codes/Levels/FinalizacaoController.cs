using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalizacaoController : MonoBehaviour
{
    public void OnMainMenu()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }

    public void OnSelectLevel()
    {
        SceneManager.LoadScene("SelecaoNiveis");
    }
}
