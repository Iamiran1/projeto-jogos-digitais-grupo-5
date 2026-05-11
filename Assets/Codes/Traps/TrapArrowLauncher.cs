using UnityEngine;

public class TrapArrowLauncher : MonoBehaviour
{
    [Header("Configurações")]
    public float arrowSpeed = 8f;
    public float arrowRange = 10f;
    public bool shootRight = true;

    [Header("Referências")]
    public GameObject arrowPrefab;
    public Transform firePoint;

    private Animator anim;
    private bool hasFired = false;
    private float fireThreshold = 0.58f; // dispara na metade da animação

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        float normalizedTime = stateInfo.normalizedTime % 1f;

        if (normalizedTime >= fireThreshold && !hasFired)
        {
            hasFired = true;
            FireArrow();
        }

        if (normalizedTime < fireThreshold)
        {
            hasFired = false;
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