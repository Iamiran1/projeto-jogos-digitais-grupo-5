using UnityEngine;

public class TrapSaw : MonoBehaviour
{
    [Header("Configurações")]
    public float moveSpeed = 2f;
    public float moveDistance = 3f;
    [Tooltip("Se marcado, move no eixo X local da serra. Se desmarcado, move no Y local.")]
    public bool moveHorizontal = true;

    [Header("Som")]
    public float distanciaMaxSom = 8f;
    [Range(0f, 1f)]
    public float volumeMaximo = 0.7f;

    private Vector3 pontoA;
    private Vector3 pontoB;
    private Vector3 destino;
    private AudioSource audioSource;
    private Transform player;

    void Start()
    {
        Vector3 direcaoLocal = moveHorizontal ? Vector3.right : Vector3.up;
        Vector3 deslocamentoNoMundo = transform.TransformDirection(direcaoLocal) * moveDistance;
        pontoA = transform.position + deslocamentoNoMundo;
        pontoB = transform.position - deslocamentoNoMundo;
        destino = pontoA;

        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 0f;
        audioSource.loop = true;
        audioSource.Play();

        // Busca o player pela tag
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        // Movimento da serra
        transform.position = Vector3.MoveTowards(transform.position, destino, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, destino) < 0.05f)
            destino = (destino == pontoA) ? pontoB : pontoA;

        // Controla volume por distância manualmente
        if (player != null)
        {
            float distancia = Vector2.Distance(transform.position, player.position);
            float volumeCalculado = Mathf.Clamp01(1f - (distancia / distanciaMaxSom));
            audioSource.volume = volumeCalculado * volumeMaximo;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 direcaoLocal = moveHorizontal ? Vector3.right : Vector3.up;
        Vector3 deslocamentoNoMundo = transform.TransformDirection(direcaoLocal) * moveDistance;
        Vector3 centro = Application.isPlaying ? (pontoA + pontoB) / 2f : transform.position;
        Vector3 pA = centro + deslocamentoNoMundo;
        Vector3 pB = centro - deslocamentoNoMundo;
        Gizmos.DrawLine(pA, pB);
        Gizmos.DrawWireSphere(pA, 0.1f);
        Gizmos.DrawWireSphere(pB, 0.1f);

        // Mostra o raio do som no Editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaMaxSom);
    }
}