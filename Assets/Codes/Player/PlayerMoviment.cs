using System.Collections.Generic;
using UnityEngine;

public class PlayerMoviment : MonoBehaviour
{
    [Header("Movimentação")]
    public float walkSpeed = 5f;
    public float runSpeed = 9f;
    public float jumpForce = 10f;

    private Rigidbody2D rb;
    private BoxCollider2D col2d;
    private PlayerPush playerPush;
    private float originalHeight;
    private PlayerAnimator playerAnimator;
    // Rastreia quais colisores estão sustentando o personagem.
    // Adiciona só contatos válidos (chão ou topo de caixa); remove em qualquer Exit.
    private readonly HashSet<Collider2D> groundContacts = new HashSet<Collider2D>();
    private bool isGrounded => groundContacts.Count > 0;
    private bool isCrouching = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col2d = GetComponent<BoxCollider2D>();
        playerPush = GetComponent<PlayerPush>();
        playerAnimator = GetComponent<PlayerAnimator>();

        originalHeight = col2d != null ? col2d.size.y : 1f;
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");

        bool crouchKeyHeld = Input.GetKey(KeyCode.S)           ||
                             Input.GetKey(KeyCode.DownArrow)   ||
                             Input.GetKey(KeyCode.LeftControl) ||
                             Input.GetKey(KeyCode.RightControl);

        // Inicia agachamento só quando está no chão; mantém enquanto a tecla estiver pressionada.
        // Não verifica isGrounded continuamente para evitar loop: agachar → collider sobe → perde chão → desagacha → collider desce → ...
        if (crouchKeyHeld && isGrounded) isCrouching = true;
        if (!crouchKeyHeld)              isCrouching = false;

        bool isPushing   = playerPush != null && playerPush.IsPushing;
        bool isRunning   = Input.GetKey(KeyCode.LeftShift) && moveX != 0 && !isCrouching && !isPushing;
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        if (isPushing) currentSpeed *= playerPush.pushSpeedMultiplier;

        rb.linearVelocity = new Vector2(moveX * currentSpeed, rb.linearVelocity.y);

        if (Input.GetButtonDown("Jump") && isGrounded && !isCrouching)
        {
            if (playerAnimator != null) playerAnimator.TriggerJump(); // ← animação primeiro
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            groundContacts.Clear(); // ← bônus: força saída do estado "no chão" imediatamente
        }

        // Agachar - reduz pelo topo mantendo os pés no lugar
        float targetScaleY = isCrouching ? 0.5f : 1f;
        if (!Mathf.Approximately(transform.localScale.y, targetScaleY))
        {
            float feetY = transform.position.y - transform.localScale.y * originalHeight / 2f;
            transform.localScale = new Vector3(transform.localScale.x, targetScaleY, 1f);
            transform.position   = new Vector3(transform.position.x, feetY + targetScaleY * originalHeight / 2f, transform.position.z);
        }

        // Virar o sprite
        if (moveX > 0) transform.localScale = new Vector3(1f,  transform.localScale.y, 1f);
        if (moveX < 0) transform.localScale = new Vector3(-1f, transform.localScale.y, 1f);
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
                if (contact.normal.y > 0.5f) { groundContacts.Add(col.collider); return; }
            }
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        groundContacts.Remove(col.collider);
    }
}
