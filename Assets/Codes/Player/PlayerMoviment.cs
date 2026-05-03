using System.Collections.Generic;
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

    private Rigidbody2D rb;
    private BoxCollider2D col2d;
    private PlayerPush playerPush;
    private PlayerAnimator playerAnimator;

    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;

    private readonly HashSet<Collider2D> groundContacts = new HashSet<Collider2D>();
    private bool isGrounded => groundContacts.Count > 0;
    private bool isCrouching = false;

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
        }
    }

    void Update()
    {
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

        if (Input.GetButtonDown("Jump") && isGrounded && !isCrouching)
        {
            if (playerAnimator != null)
                playerAnimator.TriggerJump();

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            groundContacts.Clear();
        }

        ApplyCrouchCollider();

        // Virar o sprite/Player
        if (moveX > 0)
            transform.localScale = new Vector3(1f, transform.localScale.y, 1f);

        if (moveX < 0)
            transform.localScale = new Vector3(-1f, transform.localScale.y, 1f);
    }

    private void ApplyCrouchCollider()
    {
        if (col2d == null) return;

        if (isCrouching)
        {
            float newHeight = originalColliderSize.y * crouchHeightMultiplier;
            float heightDifference = originalColliderSize.y - newHeight;

            col2d.size = new Vector2(originalColliderSize.x, newHeight);

            // Diminui o collider pelo topo, mantendo o bottom no mesmo lugar
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

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            groundContacts.Add(col.collider);
            return;
        }

        if (col.gameObject.CompareTag("Box"))
        {
            foreach (ContactPoint2D contact in col.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    groundContacts.Add(col.collider);
                    return;
                }
            }
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        groundContacts.Remove(col.collider);
    }
}