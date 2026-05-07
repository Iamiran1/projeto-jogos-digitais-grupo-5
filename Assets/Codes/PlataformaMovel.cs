using UnityEngine;

public class PlataformaMovel : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public Vector2 direcao;
    public float velocidade = 2.0f;

    private Vector2 pontoA;
    private Vector2 pontoB;
    private Vector2 destino;

    void Start()
    {
        pontoA = transform.position;
        pontoB = pontoA + direcao;
        destino = pontoB;
    }

    void Update()
    {
        // Move a plataforma
        transform.position = Vector2.MoveTowards(transform.position, destino, velocidade * Time.deltaTime);

        // Inverte a direção ao chegar no destino
        if (Vector2.Distance(transform.position, destino) < 0.1f)
        {
            destino = (destino == pontoA) ? pontoB : pontoA;
        }
    }

    // Quando algo encostar na plataforma
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica se quem encostou tem a Tag "Player"
        if (collision.gameObject.CompareTag("Player"))
        {
            // O Player vira "filho" da plataforma
            collision.transform.SetParent(transform);
        }
    }

    // Quando algo sair de cima da plataforma
    private void OnCollisionExit2D(Collision2D collision)
    {
        // Verifica se quem saiu foi o Player
        if (collision.gameObject.CompareTag("Player"))
        {
            // O Player deixa de ser filho da plataforma (volta para a raiz do jogo)
            collision.transform.SetParent(null);
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 pos = Application.isPlaying ? pontoA : (Vector2)transform.position;
        Gizmos.DrawLine(pos, pos + direcao);
    }
}