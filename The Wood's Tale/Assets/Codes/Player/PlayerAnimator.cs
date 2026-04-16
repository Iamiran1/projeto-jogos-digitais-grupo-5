using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        bool isMoving = moveX != 0;
        bool isRunning = isMoving && Input.GetKey(KeyCode.LeftShift);

        anim.SetBool("walk", isMoving && !isRunning);
        anim.SetBool("run", isRunning);
    }
}