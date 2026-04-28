using UnityEngine;

public class PlayerPush : MonoBehaviour
{
    [Header("Empurrar")]
    public float pushForce = 20f;
    [Range(0f, 1f)]
    public float pushSpeedMultiplier = 0.5f;

    public bool IsPushing { get; private set; }

    private Rigidbody2D contactedBox;

    void FixedUpdate()
    {
        if (contactedBox == null) { IsPushing = false; return; }

        float moveX = Input.GetAxisRaw("Horizontal");
        if (moveX == 0) { IsPushing = false; return; }

        contactedBox.AddForce(new Vector2(moveX * pushForce, 0f), ForceMode2D.Force);
        IsPushing = true;
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Box")) return;

        foreach (ContactPoint2D contact in col.contacts)
        {
            if (Mathf.Abs(contact.normal.x) > 0.5f)
            {
                contactedBox = col.gameObject.GetComponent<Rigidbody2D>();
                return;
            }
        }

        contactedBox = null;
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Box"))
            contactedBox = null;
    }
}
