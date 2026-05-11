using UnityEngine;

public class TrapSaw : MonoBehaviour
{
    [Header("Configurações")]
    public float moveSpeed = 2f;
    public float moveDistance = 3f;
    public bool moveHorizontal = true;

    private Vector3 startPosition;
    private float direction = 1f;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (moveHorizontal)
        {
            transform.Translate(Vector2.right * direction * moveSpeed * Time.deltaTime);
            if (Mathf.Abs(transform.position.x - startPosition.x) >= moveDistance)
                direction *= -1f;
        }
        else
        {
            transform.Translate(Vector2.up * direction * moveSpeed * Time.deltaTime);
            if (Mathf.Abs(transform.position.y - startPosition.y) >= moveDistance)
                direction *= -1f;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }
    }
}