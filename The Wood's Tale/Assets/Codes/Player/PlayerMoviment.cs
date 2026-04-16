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
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && moveX != 0;
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        // Direita e esquerda
        rb.linearVelocity = new Vector2(moveX * currentSpeed, rb.linearVelocity.y);

        // Pulo
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // Agachar
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            transform.localScale = new Vector3(1f, 0.5f, 1f);
        }
        else
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }

        // Virar o sprite
        if (moveX > 0) transform.localScale = new Vector3(1f, transform.localScale.y, 1f);
        if (moveX < 0) transform.localScale = new Vector3(-1f, transform.localScale.y, 1f);

        // Animator
        anim.SetBool("walk", moveX != 0 && !isRunning);
        anim.SetBool("run", isRunning);
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