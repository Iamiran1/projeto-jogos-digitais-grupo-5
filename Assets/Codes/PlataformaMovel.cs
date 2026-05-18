using UnityEngine;

public class PlataformaMovel : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public Vector2 direcao;
    public float velocidade = 2.0f;

    [Header("Som")]
    public AudioClip somMovendo;
    [Range(0f, 1f)]
    public float volumeMaximo = 0.7f;
    public float distanciaMaxSom = 8f;

    private Vector2 pontoA;
    private Vector2 pontoB;
    private Vector2 destino;
    private AudioSource audioSource;
    private Transform player;

    void Start()
    {
        pontoA = transform.position;
        pontoB = pontoA + direcao;
        destino = pontoB;

        audioSource = GetComponent<AudioSource>();

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, destino, velocidade * Time.deltaTime);

        if (Vector2.Distance(transform.position, destino) < 0.1f)
            destino = (destino == pontoA) ? pontoB : pontoA;

        // Debug temporário
        if (player != null)
            Debug.Log("Posição plataforma: " + transform.position + " | Posição player: " + player.position + " | Distância: " + Vector2.Distance(transform.position, player.position));

        // Controla volume por distância
        if (player != null && audioSource != null)
        {
            float distancia = Vector2.Distance(transform.position, player.position);
            float volumeCalculado = Mathf.Clamp01(1f - (distancia / distanciaMaxSom));
            audioSource.volume = volumeCalculado * volumeMaximo;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.transform != null && gameObject.activeInHierarchy)
                collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.transform != null && collision.transform.parent == transform)
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