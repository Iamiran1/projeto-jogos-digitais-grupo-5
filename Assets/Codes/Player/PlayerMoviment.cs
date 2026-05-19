using UnityEngine;

public class PlayerMoviment : MonoBehaviour
{
    [Header("Movimentação")]
    public float walkSpeed = 5f;
    public float runSpeed = 9f;
    [SerializeField] private float timeToRun = 0.8f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 0.8f;

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

    [Header("Sons")]
    public AudioClip somWallSlide;
    [Range(0f, 1f)]
    public float volumeWallSlide = 0.7f;
    public AudioClip somJump;
    [Range(0f, 1f)]
    public float volumeJump = 1f;
    public AudioClip somDash;
    [Range(0f, 1f)]
    public float volumeDash = 1f;

    private Rigidbody2D rb;
    private CapsuleCollider2D col2d;
    private PlayerPush playerPush;
    private PlayerAnimator playerAnimator;
    private PlayerDeath playerDeath;   // FIX: referência ao PlayerDeath
    private AudioSource audioSource;

    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;

    private bool isGrounded;
    private bool isCrouching;
    private bool isJumping;

    private bool isTouchingWall;
    private bool isWallSliding;
    private bool isWallJumping;

    private bool isDashing;
    private float dashTimer;
    private float dashCooldownCounter;
    private int dashDirection;

    private float runTimer;
    private int lastMoveDirection;
    private bool isRunning;

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
    public bool IsDashing => isDashing;
    public bool IsRunning => isRunning;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col2d = GetComponent<CapsuleCollider2D>();
        playerPush = GetComponent<PlayerPush>();
        playerAnimator = GetComponent<PlayerAnimator>();
        playerDeath = GetComponent<PlayerDeath>();   // FIX

        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        if (col2d != null)
        {
            originalColliderSize = col2d.size;
            originalColliderOffset = col2d.offset;
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    void Update()
    {
        // FIX: se o player está morto, NÃO processamos nada. Isso resolve a
        // race condition em que o dash/jump aplicaria velocidade residual
        // por mais um frame antes do enabled=false do PlayerDeath ter efeito.
        // Adicionalmente, zeramos a velocidade aqui como dupla segurança.
        if (playerDeath != null && playerDeath.IsDead)
        {
            if (rb != null) rb.linearVelocity = Vector2.zero;
            return;
        }
        if (MobileHUD.IsPaused) return;

        CheckGround();

        float moveX = Mathf.Clamp(Input.GetAxisRaw("Horizontal") + MobileInput.HorizontalAxis, -1f, 1f);

        CheckWall();
        HandleCrouch();
        HandleWallSlide(moveX);
        HandleJump();
        HandleDash(moveX);

        if (!isDashing)
        {
            if (wallJumpLockCounter > 0f)
                wallJumpLockCounter -= Time.deltaTime;
            else
                HandleMovement(moveX);
        }

        FlipPlayer(moveX);
        HandleWallSlideSound();
    }

    private void HandleWallSlideSound()
    {
        if (somWallSlide == null || audioSource == null) return;

        if (isWallSliding && (!audioSource.isPlaying || audioSource.clip != somWallSlide))
        {
            audioSource.clip = somWallSlide;
            audioSource.loop = true;
            audioSource.volume = volumeWallSlide;
            audioSource.Play();
        }
        else if (!isWallSliding && audioSource.clip == somWallSlide)
        {
            audioSource.Stop();
            audioSource.clip = null;
            audioSource.loop = false;
        }
    }

    private void PlayJumpSound()
    {
        if (somJump != null && audioSource != null)
            audioSource.PlayOneShot(somJump, volumeJump);
    }

    private void PlayDashSound()
    {
        if (somDash != null && audioSource != null)
            audioSource.PlayOneShot(somDash, volumeDash);
    }

    private void HandleMovement(float moveX)
    {
        if (isWallSliding)
        {
            runTimer = 0f;
            isRunning = false;
            return;
        }

        bool pressingIntoDetectedWall = isTouchingWall &&
            ((wallDirection == 1 && moveX > 0) || (wallDirection == -1 && moveX < 0));

        if (!isGrounded && moveX != 0f && (IsMovingIntoWall(moveX) || pressingIntoDetectedWall))
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        bool isPushing = playerPush != null && playerPush.IsPushing;

        if (moveX == 0f || isCrouching || isPushing)
        {
            runTimer = 0f;
            lastMoveDirection = 0;
            isRunning = false;
        }
        else
        {
            int dir = (int)Mathf.Sign(moveX);
            if (lastMoveDirection != 0 && dir != lastMoveDirection)
                runTimer = 0f;
            else if (isGrounded)
                runTimer += Time.deltaTime;
            lastMoveDirection = dir;
            isRunning = runTimer >= timeToRun;
        }

        float t = timeToRun > 0f ? Mathf.Clamp01(runTimer / timeToRun) : 1f;
        float currentSpeed = Mathf.Lerp(walkSpeed, runSpeed, t);

        if (isCrouching)
            currentSpeed *= crouchSpeedMultiplier;

        if (isPushing)
            currentSpeed *= playerPush.pushSpeedMultiplier;

        rb.linearVelocity = new Vector2(moveX * currentSpeed, rb.linearVelocity.y);
    }

    private void HandleDash(float moveX)
    {
        if (dashCooldownCounter > 0f)
            dashCooldownCounter -= Time.deltaTime;

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f || isTouchingWall)
            {
                isDashing = false;
                return;
            }
            rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0f);
            return;
        }

        bool dashDown = Input.GetKeyDown(KeyCode.LeftShift) || MobileInput.DashPressed;
        MobileInput.DashPressed = false;

        if (dashDown &&
            dashCooldownCounter <= 0f &&
            !isCrouching &&
            !isWallSliding)
        {
            isDashing = true;
            dashTimer = dashDuration;
            dashCooldownCounter = dashCooldown;
            dashDirection = moveX != 0 ? (int)Mathf.Sign(moveX) : (int)Mathf.Sign(transform.localScale.x);
            rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0f);
            PlayDashSound();
        }
    }

    private void HandleJump()
    {
        bool jumpDown = Input.GetKeyDown(KeyCode.Space) || MobileInput.JumpPressed;
        bool jumpHeld = Input.GetKey(KeyCode.Space) || MobileInput.JumpHeld;
        bool jumpUp = Input.GetKeyUp(KeyCode.Space) || MobileInput.JumpReleased;
        MobileInput.JumpPressed = false;
        MobileInput.JumpReleased = false;

        if (jumpDown && isWallSliding)
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

            PlayJumpSound();
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

        if (jumpDown)
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;

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

            PlayJumpSound();
        }

        if (jumpUp && isJumping)
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

    private bool IsMovingIntoWall(float moveX)
    {
        var contacts = new ContactPoint2D[6];
        int count = rb.GetContacts(contacts);

        float playerBottom = col2d.bounds.min.y;
        float playerHeight = col2d.bounds.size.y;
        float edgeThreshold = playerBottom + playerHeight * 0.25f;

        for (int i = 0; i < count; i++)
        {
            if (contacts[i].point.y < edgeThreshold) continue;

            float nx = contacts[i].normal.x;
            if (Mathf.Abs(nx) > 0.7f)
            {
                bool movingInto = (nx < 0f && moveX > 0f) || (nx > 0f && moveX < 0f);
                if (movingInto) return true;
            }
        }
        return false;
    }

    private void HandleCrouch()
    {
        bool crouchKeyHeld = Input.GetKey(KeyCode.S) ||
                             Input.GetKey(KeyCode.DownArrow) ||
                             MobileInput.CrouchHeld;

        if (crouchKeyHeld && isGrounded)
            isCrouching = true;
        else if (isCrouching && HasCeilingAbove())
            isCrouching = true;
        else
            isCrouching = false;

        ApplyCrouchCollider();
    }

    private bool HasCeilingAbove()
    {
        if (col2d == null) return false;

        float crouchedHeight = originalColliderSize.y * crouchHeightMultiplier;
        float checkDistance = originalColliderSize.y - crouchedHeight;

        Vector2 origin = new Vector2(col2d.bounds.center.x, col2d.bounds.max.y);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up, checkDistance, groundLayer);

        Debug.DrawRay(origin, Vector2.up * checkDistance,
            hit.collider != null ? Color.yellow : Color.white);

        return hit.collider != null;
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

        float rayLen = col2d.bounds.extents.y + groundExtraDistance;
        Vector2 center = col2d.bounds.center;
        float halfWidth = col2d.bounds.extents.x * 0.8f;

        bool hitCenter = Physics2D.Raycast(center, Vector2.down, rayLen, groundLayer).collider != null;
        bool hitLeft = Physics2D.Raycast(center + Vector2.left * halfWidth, Vector2.down, rayLen, groundLayer).collider != null;
        bool hitRight = Physics2D.Raycast(center + Vector2.right * halfWidth, Vector2.down, rayLen, groundLayer).collider != null;

        isGrounded = hitCenter || hitLeft || hitRight;

        Color c = isGrounded ? Color.green : Color.red;
        Debug.DrawRay(center, Vector2.down * rayLen, c);
        Debug.DrawRay(center + Vector2.left * halfWidth, Vector2.down * rayLen, c);
        Debug.DrawRay(center + Vector2.right * halfWidth, Vector2.down * rayLen, c);
    }

    private void CheckWall()
    {
        if (col2d == null)
        {
            isTouchingWall = false;
            return;
        }

        Vector2 center = col2d.bounds.center;
        float rayDistance = col2d.bounds.extents.x + wallCheckDistance;
        float halfHeight = col2d.bounds.extents.y * 0.8f;

        Vector2 top = center + Vector2.up * halfHeight;
        Vector2 bottom = center + Vector2.down * halfHeight;

        bool rightTop = Physics2D.Raycast(top, Vector2.right, rayDistance, wallLayer).collider != null;
        bool rightMid = Physics2D.Raycast(center, Vector2.right, rayDistance, wallLayer).collider != null;
        bool rightBottom = Physics2D.Raycast(bottom, Vector2.right, rayDistance, wallLayer).collider != null;

        bool leftTop = Physics2D.Raycast(top, Vector2.left, rayDistance, wallLayer).collider != null;
        bool leftMid = Physics2D.Raycast(center, Vector2.left, rayDistance, wallLayer).collider != null;
        bool leftBottom = Physics2D.Raycast(bottom, Vector2.left, rayDistance, wallLayer).collider != null;

        bool touchRight = rightTop || rightMid || rightBottom;
        bool touchLeft = leftTop || leftMid || leftBottom;

        if (touchRight)
        {
            isTouchingWall = true;
            wallDirection = 1;
        }
        else if (touchLeft)
        {
            isTouchingWall = true;
            wallDirection = -1;
        }
        else
        {
            isTouchingWall = false;
            wallDirection = 0;
        }

        Color cr = touchRight ? Color.cyan : Color.red;
        Color cl = touchLeft ? Color.cyan : Color.red;
        Debug.DrawRay(top, Vector2.right * rayDistance, cr);
        Debug.DrawRay(center, Vector2.right * rayDistance, cr);
        Debug.DrawRay(bottom, Vector2.right * rayDistance, cr);
        Debug.DrawRay(top, Vector2.left * rayDistance, cl);
        Debug.DrawRay(center, Vector2.left * rayDistance, cl);
        Debug.DrawRay(bottom, Vector2.left * rayDistance, cl);
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
            Vector2 futureCenter = (Vector2)transform.position + originalColliderOffset;
            Collider2D overlap = Physics2D.OverlapCapsule(
                futureCenter,
                originalColliderSize,
                col2d.direction,
                0f,
                groundLayer
            );

            if (overlap == null || overlap == col2d)
            {
                col2d.size = originalColliderSize;
                col2d.offset = originalColliderOffset;
            }
        }
    }

    private void FlipPlayer(float moveX)
    {
        if (isWallSliding && isTouchingWall)
        {
            transform.localScale = new Vector3(-wallDirection, transform.localScale.y, 1f);
            return;
        }

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
        Gizmos.DrawLine(box.bounds.center, box.bounds.center + Vector3.down * groundRayDistance);

        int direction = transform.localScale.x > 0 ? 1 : -1;
        float wallRayDistance = box.bounds.extents.x + wallCheckDistance;
        Gizmos.DrawLine(box.bounds.center, box.bounds.center + Vector3.right * direction * wallRayDistance);
    }
}