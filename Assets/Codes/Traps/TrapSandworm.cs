using UnityEngine;
using System.Collections;

public class TrapSandworm : MonoBehaviour
{
    private BoxCollider2D damageCollider;
    private Animator anim;
    private bool playerInRange = false;
    private AudioSource audioSource;
    private Transform player;

    [Header("Som")]
    public AudioClip somMinhoca;
    [Range(0f, 1f)]
    public float volume = 1f;
    public float distanciaMaxSom = 8f;

    void Start()
    {
        anim = GetComponent<Animator>();
        damageCollider = GetComponent<BoxCollider2D>();
        damageCollider.enabled = false;
        anim.speed = 0;

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
        return Mathf.Clamp01(1f - (distancia / distanciaMaxSom)) * volume;
    }

    public void PlayerEnteredRange()
    {
        playerInRange = true;
        anim.speed = 1;
    }

    public void PlayerExitedRange()
    {
        playerInRange = false;
        StartCoroutine(WaitAndStop());
    }

    private IEnumerator WaitAndStop()
    {
        if (!gameObject.activeInHierarchy) yield break;
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
        float remainingTime = (1f - state.normalizedTime % 1f) * state.length;
        yield return new WaitForSeconds(remainingTime);
        if (!playerInRange && gameObject.activeInHierarchy)
        {
            anim.speed = 0;
            damageCollider.enabled = false;
        }
    }

    // Chamado pelo Animation Event no frame do ataque
    public void EnableCollider()
    {
        damageCollider.enabled = true;

        // Toca o som no momento do ataque
        if (somMinhoca != null)
            audioSource.PlayOneShot(somMinhoca, GetVolumeByDistance());
    }

    public void DisableCollider()
    {
        damageCollider.enabled = false;
    }
}