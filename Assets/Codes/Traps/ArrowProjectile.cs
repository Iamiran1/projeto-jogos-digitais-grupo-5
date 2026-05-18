using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private float range;
    private Vector2 startPosition;

    public void Initialize(Vector2 dir, float spd, float rng)
    {
        direction = dir;
        speed = spd;
        range = rng;
        startPosition = transform.position;

        float angle = dir.x > 0 ? 0 : 180;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        if (Vector2.Distance(startPosition, transform.position) >= range)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Trap"))
            Destroy(gameObject);
    }
}