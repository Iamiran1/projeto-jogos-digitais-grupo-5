using UnityEngine;

public class TrapCeiling : MonoBehaviour
{
    [Header("Configurações")]
    public float dropSpeed = 3f;
    public float dropDistance = 3f;
    public float waitTime = 2f;

    private Vector3 startPosition;
    private bool isDropping = false;
    private bool isReturning = false;
    private float waitTimer = 0f;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (!isDropping && !isReturning)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTime)
            {
                waitTimer = 0f;
                isDropping = true;
            }
        }

        if (isDropping)
        {
            transform.Translate(Vector2.down * dropSpeed * Time.deltaTime);
            if (transform.position.y <= startPosition.y - dropDistance)
            {
                isDropping = false;
                isReturning = true;
            }
        }

        if (isReturning)
        {
            transform.Translate(Vector2.up * dropSpeed * Time.deltaTime);
            if (transform.position.y >= startPosition.y)
            {
                transform.position = startPosition;
                isReturning = false;
            }
        }
    }
}
