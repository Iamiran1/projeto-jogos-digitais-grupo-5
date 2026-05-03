using UnityEngine;
using UnityEngine.SceneManagement; // Linha obrigatória para trocar de cena

public class LevelTransition : MonoBehaviour
{
    [Header("Configuração de Fase")]
    public string nomeDaProximaCena;

    // Essa função é chamada automaticamente quando algo entra no "Trigger"
    void OnTriggerEnter2D(Collider2D outro)
    {
        // Verifica se quem tocou na bandeira foi o Player
        if (outro.CompareTag("Player"))
        {
            Debug.Log("O Player tocou a bandeira! Carregando a próxima fase...");
            SceneManager.LoadScene(nomeDaProximaCena);
        }
    }
}