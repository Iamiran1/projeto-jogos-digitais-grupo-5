using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private bool isGrounded;

    [Header("Chão")]
    public float rayDistance = 0.3f;
    public LayerMask groundLayer;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        bool isMoving = moveX != 0;
        bool isRunning = isMoving && Input.GetKey(KeyCode.LeftShift);
        bool isCrouching = isGrounded && (
            Input.GetKey(KeyCode.LeftControl)  ||
            Input.GetKey(KeyCode.RightControl) ||
            Input.GetKey(KeyCode.S)            ||
            Input.GetKey(KeyCode.DownArrow)
        );
        bool isFalling = rb.linearVelocity.y < -0.1f && !isCrouching;
        bool isJumping = rb.linearVelocity.y > 0.1f  && !isCrouching;

        bool nearGround = Physics2D.Raycast(transform.position, Vector2.down, rayDistance, groundLayer);

        if (isJumping || isFalling)
            isGrounded = false;

        if (isJumping)
        {
            anim.SetBool("jump",       true);
            anim.SetBool("fall",       false);
            anim.SetBool("walk",       false);
            anim.SetBool("run",        false);
            anim.SetBool("ground",     false);
            anim.SetBool("squat",      false);
            anim.SetBool("squat_walk", false);
        }
        else if (isFalling && !nearGround)
        {
            anim.SetBool("fall",       true);
            anim.SetBool("jump",       false);
            anim.SetBool("walk",       false);
            anim.SetBool("run",        false);
            anim.SetBool("ground",     false);
            anim.SetBool("squat",      false);
            anim.SetBool("squat_walk", false);
        }
        else if (isFalling && nearGround)
        {
            anim.SetBool("ground",     true);
            anim.SetBool("fall",       false);
            anim.SetBool("jump",       false);
            anim.SetBool("walk",       false);
            anim.SetBool("squat",      false);
            anim.SetBool("squat_walk", false);
        }
        else if (isGrounded && isCrouching)
        {
            anim.SetBool("squat",      true);
            anim.SetBool("squat_walk", isMoving);
            anim.SetBool("walk",       false);
            anim.SetBool("run",        false);
            anim.SetBool("jump",       false);
            anim.SetBool("fall",       false);
            anim.SetBool("ground",     false);
        }
        else if (isGrounded)
        {
            anim.SetBool("jump",       false);
            anim.SetBool("fall",       false);
            anim.SetBool("ground",     true);
            anim.SetBool("squat",      false);
            anim.SetBool("squat_walk", false);
            anim.SetBool("walk",       isMoving);
            anim.SetBool("run",        isRunning);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }
}