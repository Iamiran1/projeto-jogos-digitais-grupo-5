using UnityEngine;

public class PlayerMoviment : MonoBehaviour
{
    [Header("Movimentação")]
    public float walkSpeed = 5f;
    public float runSpeed = 9f;
    public float jumpForce = 10f;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");

        bool isCrouching = Input.GetKey(KeyCode.S)            ||
                           Input.GetKey(KeyCode.DownArrow)    ||
                           Input.GetKey(KeyCode.LeftControl)  ||
                           Input.GetKey(KeyCode.RightControl);

        bool isRunning = Input.GetKey(KeyCode.LeftShift) && moveX != 0 && !isCrouching;
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        rb.linearVelocity = new Vector2(moveX * currentSpeed, rb.linearVelocity.y);

        if (Input.GetButtonDown("Jump") && isGrounded && !isCrouching)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // Agachar
        if (isCrouching)
            transform.localScale = new Vector3(transform.localScale.x, 0.5f, 1f);
        else
            transform.localScale = new Vector3(transform.localScale.x, 1f, 1f);

        // Virar o sprite
        if (moveX > 0) transform.localScale = new Vector3(1f,  transform.localScale.y, 1f);
        if (moveX < 0) transform.localScale = new Vector3(-1f, transform.localScale.y, 1f);

        anim.SetBool("walk", moveX != 0 && !isRunning && !isCrouching);
        anim.SetBool("run",  isRunning);
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