using UnityEngine;

public class PlayerPush : MonoBehaviour
{
    [Header("Empurrar")]
    public float pushForce = 20f;

    [Range(0f, 1f)]
    public float pushSpeedMultiplier = 0.5f;

    [Header("Collider ao empurrar")]
    [SerializeField] private CapsuleCollider2D playerCollider;

    [SerializeField] private Vector2 normalSize = new Vector2(0.75f, 1.55f);
    [SerializeField] private Vector2 normalOffset = new Vector2(0f, 0.05f);

    [SerializeField] private Vector2 pushingSize = new Vector2(0.95f, 1.55f);
    [SerializeField] private float pushingOffsetX = 0.1f;

    public bool IsPushing { get; private set; }

    private Rigidbody2D contactedBox;
    private int facingDirection = 1;

    void Awake()
    {
        if (playerCollider == null)
            playerCollider = GetComponent<CapsuleCollider2D>();

        if (playerCollider != null)
        {
            normalSize = playerCollider.size;
            normalOffset = playerCollider.offset;
        }
    }

    void FixedUpdate()
    {
        float moveX = Input.GetAxisRaw("Horizontal");

        if (moveX > 0) facingDirection = 1;
        else if (moveX < 0) facingDirection = -1;

        if (contactedBox == null || moveX == 0)
        {
            SetPushing(false);
            return;
        }

        contactedBox.AddForce(new Vector2(moveX * pushForce, 0f), ForceMode2D.Force);
        SetPushing(true);
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
        {
            contactedBox = null;
            SetPushing(false);
        }
    }

    private void SetPushing(bool pushing)
    {
        IsPushing = pushing;

        if (playerCollider == null) return;

        if (pushing)
        {
            playerCollider.size = pushingSize;
            playerCollider.offset = new Vector2(
                normalOffset.x + pushingOffsetX,
                normalOffset.y
            );
        }
        // Quando não está empurrando, PlayerMoviment cuida do collider
    }
}