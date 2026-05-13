using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipalController : MonoBehaviour
{
    public void OnStartButton()
    {
        SceneManager.LoadScene("SelecaoNiveis");
    }
}
