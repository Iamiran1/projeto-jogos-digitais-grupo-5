using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private PlayerPush playerPush;
    private PlayerMoviment playerMoviment;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerPush = GetComponent<PlayerPush>();
        playerMoviment = GetComponent<PlayerMoviment>();
    }

    void Update()
    {
        if (anim == null || rb == null || playerMoviment == null) return;

        float moveX = Input.GetAxisRaw("Horizontal");

        bool isGrounded = playerMoviment.IsGrounded;
        bool isCrouching = playerMoviment.IsCrouching;
        bool isSliding = playerMoviment.IsWallSliding;

        bool isMoving = moveX != 0;
        bool isDashing = playerMoviment.IsDashing;
        bool isRunning = playerMoviment.IsRunning;
        bool isPushing = playerPush != null && playerPush.IsPushing;

        bool isJumping = rb.linearVelocity.y > 0.1f && !isGrounded && !isSliding;
        bool isFalling = rb.linearVelocity.y < -0.1f && !isGrounded && !isSliding;

        if (isDashing)
        {
            SetState(dash: true);
        }
        else if (isSliding)
        {
            SetState(sliding: true);
        }
        else if (isJumping)
        {
            SetState(jump: true);
        }
        else if (isFalling)
        {
            SetState(fall: true);
        }
        else if (isGrounded && isCrouching)
        {
            SetState(
                squat: !isMoving,
                squatWalk: isMoving
            );
        }
        else if (isGrounded)
        {
            SetState(
                ground: !isMoving && !isPushing,
                walk: isMoving && !isRunning && !isPushing,
                run: isRunning && !isPushing,
                push: isPushing
            );
        }
    }

    private void SetState(
        bool jump = false,
        bool fall = false,
        bool walk = false,
        bool run = false,
        bool ground = false,
        bool squat = false,
        bool squatWalk = false,
        bool push = false,
        bool sliding = false,
        bool dash = false
    )
    {
        anim.SetBool("jump", jump);
        anim.SetBool("fall", fall);
        anim.SetBool("walk", walk);
        anim.SetBool("run", run);
        anim.SetBool("ground", ground);
        anim.SetBool("squat", squat);
        anim.SetBool("squat_walk", squatWalk);
        anim.SetBool("push", push);
        anim.SetBool("sliding", sliding);
        anim.SetBool("dash", dash);
    }

    public void TriggerJump()
    {
        SetState(jump: true);
    }
}