using System.Collections;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    private Rigidbody2D rb;
    private AudioSource audioSource;
    private Transform player;

    [Header("Configurações")]
    public float fallDelay = 0.01f;
    public float destroyDelay = 2f;

    [Header("Som")]
    public AudioClip somQueda;
    public float distanciaMaxSom = 8f;
    [Range(0f, 1f)]
    public float volumeMaximo = 0.7f;

    private bool isFalling = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake = false;
        audioSource.loop = false;

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    private float GetVolumeByDistance()
    {
        if (player == null) return 0f;
        float distancia = Vector2.Distance(transform.position, player.position);
        return Mathf.Clamp01(1f - (distancia / distanciaMaxSom)) * volumeMaximo;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isFalling)
        {
            if (collision.contacts[0].normal.y < -0.5f)
            {
                StartCoroutine(Fall());
            }
        }
    }

    IEnumerator Fall()
    {
        isFalling = true;

        // Toca o som quando começa a cair
        if (somQueda != null)
            audioSource.PlayOneShot(somQueda, GetVolumeByDistance());

        yield return new WaitForSeconds(fallDelay);

        rb.bodyType = RigidbodyType2D.Dynamic;
        Destroy(gameObject, destroyDelay);
    }
}