using UnityEngine;

public class PlayerMoviment : MonoBehaviour
{
    [Header("Movimentação")]
    public float walkSpeed = 5f;
    public float runSpeed = 9f;
    public float jumpForce = 10f;

    private Rigidbody2D rb;
    private PlayerPush playerPush;
    private bool isGrounded;
    private float originalHeight;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerPush = GetComponent<PlayerPush>();
        var sr = GetComponent<SpriteRenderer>();
        originalHeight = sr != null ? sr.bounds.size.y : 1f;
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");

        bool isCrouching = isGrounded && (
                           Input.GetKey(KeyCode.S)            ||
                           Input.GetKey(KeyCode.DownArrow)    ||
                           Input.GetKey(KeyCode.LeftControl)  ||
                           Input.GetKey(KeyCode.RightControl));

        bool isPushing   = playerPush != null && playerPush.IsPushing;
        bool isRunning   = Input.GetKey(KeyCode.LeftShift) && moveX != 0 && !isCrouching && !isPushing;
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        if (isPushing) currentSpeed *= playerPush.pushSpeedMultiplier;

        rb.linearVelocity = new Vector2(moveX * currentSpeed, rb.linearVelocity.y);

        if (Input.GetButtonDown("Jump") && isGrounded && !isCrouching)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // Agachar - reduz pelo topo mantendo os pés no lugar
        float targetScaleY = isCrouching ? 0.5f : 1f;
        if (!Mathf.Approximately(transform.localScale.y, targetScaleY))
        {
            float feetY = transform.position.y - transform.localScale.y * originalHeight / 2f;
            transform.localScale = new Vector3(transform.localScale.x, targetScaleY, 1f);
            transform.position = new Vector3(transform.position.x, feetY + targetScaleY * originalHeight / 2f, transform.position.z);
        }

        // Virar o sprite
        if (moveX > 0) transform.localScale = new Vector3(1f,  transform.localScale.y, 1f);
        if (moveX < 0) transform.localScale = new Vector3(-1f, transform.localScale.y, 1f);

    }

    void FixedUpdate()
    {
        isGrounded = false;
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            return;
        }
        if (col.gameObject.CompareTag("Box"))
        {
            foreach (ContactPoint2D contact in col.contacts)
            {
                if (contact.normal.y > 0.5f) { isGrounded = true; return; }
            }
        }
    }
}