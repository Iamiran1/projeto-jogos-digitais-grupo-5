using UnityEngine;

public class TrapSandworm : MonoBehaviour
{
    private Collider2D damageCollider;
    private Animator anim;
    private bool playerInRange = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        // Pega o Box Collider 2D como collider de dano
        damageCollider = GetComponent<BoxCollider2D>();
        damageCollider.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            anim.enabled = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            anim.enabled = false;
        }
    }

    // Chamado pelo Animation Event
    public void EnableCollider()
    {
        damageCollider.enabled = true;
    }

    // Chamado pelo Animation Event
    public void DisableCollider()
    {
        damageCollider.enabled = false;
    }
}