using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private PlayerPush playerPush;
    private PlayerMoviment playerMoviment;
    private PlayerDeath playerDeath;   // FIX: referência ao PlayerDeath p/ sincronizar estado
    private bool isDead = false;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerPush = GetComponent<PlayerPush>();
        playerMoviment = GetComponent<PlayerMoviment>();
        playerDeath = GetComponent<PlayerDeath>();   // FIX
    }

    void Update()
    {
        if (anim == null || rb == null || playerMoviment == null) return;

        // FIX (sincronização): se o PlayerDeath está morto e o nosso isDead
        // ainda não foi setado (caso TriggerDeath não tenha sido chamado
        // por algum motivo), forçamos o estado morto aqui também.
        // Isso elimina o "estado intermediário" entre vivo e morto.
        if (playerDeath != null && playerDeath.IsDead)
        {
            if (!isDead)
                TriggerDeath();
            return;
        }

        if (isDead) return;

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
        // FIX: trava de segurança — depois de morto, NUNCA mais mudamos bools.
        // Isso impede que qualquer chamada residual reescreva o Animator.
        if (isDead) return;

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
        if (isDead) return;   // FIX: não dispara jump depois de morto
        SetState(jump: true);
    }

    // FIX CRÍTICO: o bug do "flicker entre vivo e morto" estava aqui.
    // Quando o player morria no meio do dash/walk/jump, os bools "dash",
    // "walk", "jump" continuavam true no Animator. O trigger "die" disparava,
    // mas a transição podia entrar em conflito com os bools ainda ativos,
    // causando troca rápida entre estados ou trava em estado intermediário.
    //
    // Solução: RESETAR TODOS OS BOOLS para false ANTES de disparar o trigger.
    // Assim o Animator vai do estado "limpo" direto para "dead" sem ambiguidade.
    public void TriggerDeath()
    {
        if (isDead) return;   // idempotente — chamadas duplas são ignoradas
        isDead = true;

        if (anim == null) return;

        // 1) Zera todos os bools — garante que o Animator não fique preso
        //    em transição (dash/walk/jump) enquanto tentamos ir pra dead.
        anim.SetBool("jump", false);
        anim.SetBool("fall", false);
        anim.SetBool("walk", false);
        anim.SetBool("run", false);
        anim.SetBool("ground", false);
        anim.SetBool("squat", false);
        anim.SetBool("squat_walk", false);
        anim.SetBool("push", false);
        anim.SetBool("sliding", false);
        anim.SetBool("dash", false);

        // 2) Limpa qualquer trigger "die" residual antes de disparar — evita
        //    double-fire caso o método seja chamado mais de uma vez.
        anim.ResetTrigger("die");
        anim.SetTrigger("die");
    }
}
