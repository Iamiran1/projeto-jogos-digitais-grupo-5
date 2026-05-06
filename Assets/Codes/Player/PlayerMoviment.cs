using UnityEngine;

public class PlayerMoviment : MonoBehaviour
{
    [Header("Movimentação")]
    public float walkSpeed = 5f;
    public float runSpeed = 9f;

    [Header("Pulo")]
    public float jumpForce = 10f;

    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float jumpBufferTime = 0.1f;

    [Range(0f, 1f)]
    [SerializeField] private float variableJumpMultiplier = 0.5f;

    [Header("Agachamento")]
    [Range(0f, 1f)]
    public float crouchSpeedMultiplier = 0.45f;

    [Range(0.1f, 1f)]
    public float crouchHeightMultiplier = 0.5f;

    [Header("Detecção de chão")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundExtraDistance = 0.08f;

    [Header("Wall Parkour")]
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallCheckDistance = 0.15f;
    [SerializeField] private float wallSlideSpeed = 1.5f;
    [SerializeField] private Vector2 wallJumpForce = new Vector2(8f, 10f);
    [SerializeField] private float wallJumpLockTime = 0.15f;
    [SerializeField] private float wallCoyoteTime = 0.15f;

    private Rigidbody2D rb;
    private CapsuleCollider2D col2d;
    private PlayerPush playerPush;
    private PlayerAnimator playerAnimator;

    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;

    private bool isGrounded;
    private bool isCrouching;
    private bool isJumping;

    private bool isTouchingWall;
    private bool isWallSliding;
    private bool isWallJumping;

    private int wallDirection;
    private int lastWallDirection;
    private float wallJumpLockCounter;
    private float wallCoyoteTimeCounter;

    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    public bool IsGrounded => isGrounded;
    public bool IsCrouching => isCrouching;
    public bool IsJumping => isJumping;
    public bool IsWallSliding => isWallSliding;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col2d = GetComponent<CapsuleCollider2D>();
        playerPush = GetComponent<PlayerPush>();
        playerAnimator = GetComponent<PlayerAnimator>();

        if (col2d != null)
        {
            originalColliderSize = col2d.size;
            originalColliderOffset = col2d.offset;
        }
    }

    void Update()
    {
        CheckGround();

        float moveX = Input.GetAxisRaw("Horizontal");

        CheckWall();
        HandleCrouch();
        HandleWallSlide(moveX);
        HandleJump();

        if (wallJumpLockCounter > 0f)
        {
            wallJumpLockCounter -= Time.deltaTime;
        }
        else
        {
            HandleMovement(moveX);
        }

        FlipPlayer(moveX);
    }

    private void HandleMovement(float moveX)
    {
        if (isWallSliding) return;

        bool isPushing = playerPush != null && playerPush.IsPushing;

        bool isRunning = Input.GetKey(KeyCode.LeftShift) &&
                         moveX != 0 &&
                         !isCrouching &&
                         !isPushing &&
                         !isWallSliding;

        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        if (isCrouching)
            currentSpeed *= crouchSpeedMultiplier;

        if (isPushing)
            currentSpeed *= playerPush.pushSpeedMultiplier;

        rb.linearVelocity = new Vector2(moveX * currentSpeed, rb.linearVelocity.y);
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isWallSliding)
        {
            isWallJumping = true;
            isWallSliding = false;
            isJumping = true;

            wallCoyoteTimeCounter = 0f;
            wallJumpLockCounter = wallJumpLockTime;

            int jumpDir = isTouchingWall ? wallDirection : lastWallDirection;
            rb.linearVelocity = new Vector2(-jumpDir * wallJumpForce.x, wallJumpForce.y);

            if (playerAnimator != null)
                playerAnimator.TriggerJump();

            return;
        }

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            isJumping = false;
            isWallJumping = false;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter > 0f &&
            coyoteTimeCounter > 0f &&
            !isCrouching &&
            !isWallSliding)
        {
            if (playerAnimator != null)
                playerAnimator.TriggerJump();

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            isJumping = true;
            isGrounded = false;

            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
        }

        if (Input.GetKeyUp(KeyCode.Space) && isJumping)
        {
            if (rb.linearVelocity.y > 0f)
            {
                rb.linearVelocity = new Vector2(
                    rb.linearVelocity.x,
                    rb.linearVelocity.y * variableJumpMultiplier
                );
            }

            isJumping = false;
        }
    }

    private void HandleCrouch()
    {
        bool crouchKeyHeld = Input.GetKey(KeyCode.S) ||
                             Input.GetKey(KeyCode.DownArrow) ||
                             Input.GetKey(KeyCode.LeftControl) ||
                             Input.GetKey(KeyCode.RightControl);

        if (crouchKeyHeld && isGrounded)
            isCrouching = true;
        else
            isCrouching = false;

        ApplyCrouchCollider();
    }

    private void HandleWallSlide(float moveX)
    {
        bool pressingIntoWall =
            (wallDirection == 1 && moveX > 0) ||
            (wallDirection == -1 && moveX < 0);

        bool activelySliding = isTouchingWall &&
                               !isGrounded &&
                               pressingIntoWall &&
                               rb.linearVelocity.y <= 0f &&
                               !isCrouching;

        if (activelySliding)
        {
            lastWallDirection = wallDirection;
            wallCoyoteTimeCounter = wallCoyoteTime;
            isWallSliding = true;
            rb.linearVelocity = new Vector2(0f, -wallSlideSpeed);
        }
        else if (wallCoyoteTimeCounter > 0f && !isGrounded)
        {
            wallCoyoteTimeCounter -= Time.deltaTime;
            isWallSliding = true;
            // fora da parede: deixa a gravidade agir normalmente
        }
        else
        {
            wallCoyoteTimeCounter = 0f;
            isWallSliding = false;
        }
    }

    private void CheckGround()
    {
        if (col2d == null)
        {
            isGrounded = false;
            return;
        }

        float distanceToGround = col2d.bounds.extents.y;

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
    }

private void CheckWall()
{
    if (col2d == null)
    {
        isTouchingWall = false;
        Debug.Log("CheckWall: col2d está null");
        return;
    }

    Vector2 origin = col2d.bounds.center;
    float rayDistance = col2d.bounds.extents.x + wallCheckDistance;

    RaycastHit2D hitRight = Physics2D.Raycast(
        origin,
        Vector2.right,
        rayDistance,
        wallLayer
    );

    RaycastHit2D hitLeft = Physics2D.Raycast(
        origin,
        Vector2.left,
        rayDistance,
        wallLayer
    );

    if (hitRight.collider != null)
    {
        isTouchingWall = true;
        wallDirection = 1;

        Debug.Log("Parede detectada na DIREITA: " + hitRight.collider.gameObject.name);
    }
    else if (hitLeft.collider != null)
    {
        isTouchingWall = true;
        wallDirection = -1;

        Debug.Log("Parede detectada na ESQUERDA: " + hitLeft.collider.gameObject.name);
    }
    else
    {
        isTouchingWall = false;
        wallDirection = 0;

        Debug.Log("Nenhuma parede detectada");
    }

    Debug.DrawRay(
        origin,
        Vector2.right * rayDistance,
        hitRight.collider != null ? Color.cyan : Color.red
    );

    Debug.DrawRay(
        origin,
        Vector2.left * rayDistance,
        hitLeft.collider != null ? Color.cyan : Color.red
    );
}

    private void ApplyCrouchCollider()
    {
        if (col2d == null) return;

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

    private void FlipPlayer(float moveX)
    {
        if (isWallSliding && isTouchingWall)
        {
            // Vira para longe da parede automaticamente
            transform.localScale = new Vector3(-wallDirection, transform.localScale.y, 1f);
            return;
        }

        // Durante o coyote time (acabou de sair da parede) ou no chão: vira normal
        if (moveX > 0)
            transform.localScale = new Vector3(1f, transform.localScale.y, 1f);

        if (moveX < 0)
            transform.localScale = new Vector3(-1f, transform.localScale.y, 1f);
    }

    private void OnDrawGizmosSelected()
    {
        CapsuleCollider2D box = GetComponent<CapsuleCollider2D>();
        if (box == null) return;

        float groundRayDistance = box.bounds.extents.y + groundExtraDistance;

        Gizmos.DrawLine(
            box.bounds.center,
            box.bounds.center + Vector3.down * groundRayDistance
        );

        int direction = transform.localScale.x > 0 ? 1 : -1;
        float wallRayDistance = box.bounds.extents.x + wallCheckDistance;

        Gizmos.DrawLine(
            box.bounds.center,
            box.bounds.center + Vector3.right * direction * wallRayDistance
        );
    }
}