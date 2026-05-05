using UnityEngine;

public class PlayerMoviment : MonoBehaviour
{
    [Header("Movimentação")]
    public float walkSpeed = 5f;
    public float runSpeed = 9f;
    public float jumpForce = 10f;

    [Header("Agachamento")]
    [Range(0f, 1f)]
    public float crouchSpeedMultiplier = 0.45f;

    [Range(0.1f, 1f)]
    public float crouchHeightMultiplier = 0.5f;

    [Header("Detecção de chão")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundExtraDistance = 0.08f;

    private Rigidbody2D rb;
    private BoxCollider2D col2d;
    private PlayerPush playerPush;
    private PlayerAnimator playerAnimator;

    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;

    private bool isGrounded;
    private bool isCrouching = false;

    public bool IsGrounded => isGrounded;
    public bool IsCrouching => isCrouching;
    private float distanceToGround;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col2d = GetComponent<BoxCollider2D>();
        playerPush = GetComponent<PlayerPush>();
        playerAnimator = GetComponent<PlayerAnimator>();

        if (col2d != null)
        {
            originalColliderSize = col2d.size;
            originalColliderOffset = col2d.offset;

            distanceToGround = col2d.bounds.extents.y;
        }
    }

    void Update()
    {
        CheckGround();

        float moveX = Input.GetAxisRaw("Horizontal");

        bool crouchKeyHeld = Input.GetKey(KeyCode.S) ||
                             Input.GetKey(KeyCode.DownArrow) ||
                             Input.GetKey(KeyCode.LeftControl) ||
                             Input.GetKey(KeyCode.RightControl);

        if (crouchKeyHeld && isGrounded)
            isCrouching = true;

        if (!crouchKeyHeld)
            isCrouching = false;

        bool isPushing = playerPush != null && playerPush.IsPushing;

        bool isRunning = Input.GetKey(KeyCode.LeftShift) &&
                         moveX != 0 &&
                         !isCrouching &&
                         !isPushing;

        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        if (isCrouching)
            currentSpeed *= crouchSpeedMultiplier;

        if (isPushing)
            currentSpeed *= playerPush.pushSpeedMultiplier;

        rb.linearVelocity = new Vector2(moveX * currentSpeed, rb.linearVelocity.y);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isCrouching)
        {
            if (playerAnimator != null)
                playerAnimator.TriggerJump();

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        ApplyCrouchCollider();

        if (moveX > 0)
            transform.localScale = new Vector3(1f, transform.localScale.y, 1f);

        if (moveX < 0)
            transform.localScale = new Vector3(-1f, transform.localScale.y, 1f);
    }

    private void CheckGround()
    {
        if (col2d == null)
        {
            isGrounded = false;
            return;
        }

        distanceToGround = col2d.bounds.extents.y;

        RaycastHit2D hit = Physics2D.Raycast(
            col2d.bounds.center,
            Vector2.down,
            distanceToGround + groundExtraDistance,
            groundLayer
        );

        isGrounded = hit.collider != null;

        Debug.DrawRay(
            col2d.bounds.center,
            Vector2.down * (distanceToGround + groundExtraDistance),
            isGrounded ? Color.green : Color.red
        );

        Debug.Log("Grounded: " + isGrounded + " | Hit: " +
                  (hit.collider != null ? hit.collider.gameObject.name : "Nada"));
    }

    void FixedUpdate()
    {
        ApplyCrouchCollider();
    }

    public void ApplyCrouchCollider()
    {
        if (col2d == null) return;

        bool isPushing = playerPush != null && playerPush.IsPushing;
        if (isPushing) return;

        if (isCrouching)
        {
            float newHeight = originalColliderSize.y * crouchHeightMultiplier;
            float heightDifference = originalColliderSize.y - newHeight;

            col2d.size = new Vector2(originalColliderSize.x, newHeight);

            col2d.offset = new Vector2(
                originalColliderOffset.x,
                originalColliderOffset.y - heightDifference / 2f
            );
        }
        else
        {
            col2d.size = originalColliderSize;
            col2d.offset = originalColliderOffset;
        }
    }

    private void OnDrawGizmosSelected()
    {
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box == null) return;

        float rayDistance = box.bounds.extents.y + groundExtraDistance;

        Gizmos.DrawLine(
            box.bounds.center,
            box.bounds.center + Vector3.down * rayDistance
        );
    }
}