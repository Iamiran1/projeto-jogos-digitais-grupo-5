using UnityEngine;
using System.Collections;

public class TrapArrowLauncher : MonoBehaviour
{
    [Header("Configurações")]
    public float arrowSpeed = 8f;
    public float arrowRange = 10f;
    public bool shootRight = true;

    [Header("Referências")]
    public GameObject arrowPrefab;
    public Transform firePoint;

    [Header("Som")]
    public AudioClip somFlecha;
    public float antecipacaoSom = 0.15f; // ajuste esse valor até sincronizar

    private Animator anim;
    private bool hasFired = false;
    private bool hasPlayedSound = false;
    private float fireThreshold = 0.58f;
    private AudioSource audioSource;

    void Start()
    {
        anim = GetComponent<Animator>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.maxDistance = 15f;
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        float normalizedTime = stateInfo.normalizedTime % 1f;

        // Toca o som um pouco antes da flecha
        float soundThreshold = fireThreshold - antecipacaoSom;
        if (normalizedTime >= soundThreshold && !hasPlayedSound)
        {
            hasPlayedSound = true;
            if (somFlecha != null)
                audioSource.PlayOneShot(somFlecha);
        }

        // Instancia a flecha no threshold original
        if (normalizedTime >= fireThreshold && !hasFired)
        {
            hasFired = true;
            FireArrow();
        }

        if (normalizedTime < soundThreshold)
        {
            hasFired = false;
            hasPlayedSound = false;
        }
    }

    private void FireArrow()
    {
        if (arrowPrefab != null && firePoint != null)
        {
            GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
            ArrowProjectile arrowScript = arrow.GetComponent<ArrowProjectile>();
            if (arrowScript != null)
            {
                arrowScript.Initialize(
                    shootRight ? Vector2.right : Vector2.left,
                    arrowSpeed,
                    arrowRange);
            }
        }
    }
}