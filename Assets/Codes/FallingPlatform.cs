using System.Collections;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Configurações")]
    public float fallDelay = 0.01f; // Tempo em segundos que ela treme/espera antes de cair
    public float destroyDelay = 2f; // Tempo para ser deletada depois de cair (para não pesar o jogo)

    private bool isFalling = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Se quem tocou foi o Player e a plataforma ainda não está caindo
        if (collision.gameObject.CompareTag("Player") && !isFalling)
        {
            // Verifica se o player encostou por cima (pisou) e não bateu a cabeça por baixo
            if (collision.contacts[0].normal.y < -0.5f)
            {
                StartCoroutine(Fall());
            }
        }
    }

    IEnumerator Fall()
    {
        isFalling = true;

        // Aqui você pode colocar uma animação de "tremer" no futuro, se quiser!

        // Espera o tempo definido
        yield return new WaitForSeconds(fallDelay);

        // Muda o corpo para Dinâmico, fazendo a gravidade puxar ela para baixo na hora!
        rb.bodyType = RigidbodyType2D.Dynamic;

        // Destrói o objeto depois de um tempo para não encher o "limbo" do jogo de lixo
        Destroy(gameObject, destroyDelay);
    }
}