using UnityEngine;

public class TrapSaw : MonoBehaviour
{
    [Header("Configurações")]
    public float moveSpeed = 2f;
    public float moveDistance = 3f;
    [Tooltip("Se marcado, move no eixo X local da serra. Se desmarcado, move no Y local.")]
    public bool moveHorizontal = true;

    private Vector3 pontoA;
    private Vector3 pontoB;
    private Vector3 destino;

    void Start()
    {
        // Define qual eixo local vamos usar (X ou Y)
        Vector3 direcaoLocal = moveHorizontal ? Vector3.right : Vector3.up;

        // Converte a direção local para global, respeitando a rotação do objeto na Unity
        Vector3 deslocamentoNoMundo = transform.TransformDirection(direcaoLocal) * moveDistance;

        // O seu script original oscilava entre (posicao + distancia) e (posicao - distancia)
        // Então criamos esses dois pontos no mundo real para a serra viajar entre eles
        pontoA = transform.position + deslocamentoNoMundo;
        pontoB = transform.position - deslocamentoNoMundo;

        // Começa o movimento em direção ao ponto A
        destino = pontoA;
    }

    void Update()
    {
        // Move a serra no mundo usando a matemática corrigida
        transform.position = Vector3.MoveTowards(transform.position, destino, moveSpeed * Time.deltaTime);

        // Verifica a distância absoluta até o destino (funciona em qualquer ângulo)
        if (Vector3.Distance(transform.position, destino) < 0.05f)
        {
            // Ao chegar num lado, inverte para o outro
            destino = (destino == pontoA) ? pontoB : pontoA;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }
    }

    // Adicionei essa função para ajudar no seu Level Design!
    // Ela vai desenhar uma linha vermelha no Editor mostrando o trajeto exato da serra.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Vector3 direcaoLocal = moveHorizontal ? Vector3.right : Vector3.up;
        Vector3 deslocamentoNoMundo = transform.TransformDirection(direcaoLocal) * moveDistance;

        // Se o jogo estiver rodando, mantemos a linha travada nos pontos. Se não, desenhamos a partir da posição atual.
        Vector3 centro = Application.isPlaying ? (pontoA + pontoB) / 2f : transform.position;
        Vector3 pA = centro + deslocamentoNoMundo;
        Vector3 pB = centro - deslocamentoNoMundo;

        Gizmos.DrawLine(pA, pB);
        Gizmos.DrawWireSphere(pA, 0.1f);
        Gizmos.DrawWireSphere(pB, 0.1f);
    }
}