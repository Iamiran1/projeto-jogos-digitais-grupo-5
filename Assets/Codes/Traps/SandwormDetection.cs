using UnityEngine;

public class SandwormDetection : MonoBehaviour
{
    private TrapSandworm sandworm;

    void Start()
    {
        sandworm = GetComponentInParent<TrapSandworm>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            sandworm.PlayerEnteredRange();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && sandworm.gameObject.activeInHierarchy)
            sandworm.PlayerExitedRange();
    }
}