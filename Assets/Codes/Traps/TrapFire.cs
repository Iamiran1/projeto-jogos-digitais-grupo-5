using UnityEngine;

public class TrapFire : MonoBehaviour
{
    private Collider2D col;
    private AudioSource audioSource;
    private Transform player;

    [Header("Som")]
    public AudioClip somFogo;
    public float distanciaMaxSom = 8f;
    [Range(0f, 1f)]
    public float volumeMaximo = 0.7f;

    void Start()
    {
        col = GetComponent<Collider2D>();
        col.enabled = false;

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

    public void EnableCollider()
    {
        col.enabled = true;
        if (somFogo != null)
            audioSource.PlayOneShot(somFogo, GetVolumeByDistance());
    }

    public void DisableCollider()
    {
        col.enabled = false;
    }
}