using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Alvo")]
    public Transform target;

    [Header("Seguimento")]
    public Vector2 offset = new Vector2(0f, 1f);
    [SerializeField] private float smoothTimeX = 0.25f;
    [SerializeField] private float smoothTimeY = 0.15f;

    [Header("Progresso")]
    [SerializeField] private bool lockBackward = true;

    [Header("Limites do Mapa")]
    public bool useBounds = false;
    public Vector2 minBounds;
    public Vector2 maxBounds;

    private float camHalfHeight;
    private float camHalfWidth;
    private float standingHalfHeight;

    private float velX;
    private float velY;
    private float furthestX;

    void Start()
    {
        var cam = GetComponent<Camera>();
        camHalfHeight = cam.orthographicSize;
        camHalfWidth  = camHalfHeight * cam.aspect;

        if (target != null)
        {
            var sr = target.GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
                standingHalfHeight = sr.bounds.size.y / 2f;
        }

        furthestX = transform.position.x;
    }

    void LateUpdate()
    {
        if (target == null) return;

        float scaleY  = target.localScale.y;
        float targetY = target.position.y + standingHalfHeight * (1f - scaleY);

        float desiredX = target.position.x + offset.x;
        float desiredY = targetY + offset.y;

        if (lockBackward)
        {
            furthestX = Mathf.Max(furthestX, desiredX);
            desiredX  = furthestX;
        }

        if (useBounds)
        {
            desiredX = Mathf.Clamp(desiredX, minBounds.x + camHalfWidth,  maxBounds.x - camHalfWidth);
            desiredY = Mathf.Clamp(desiredY, minBounds.y + camHalfHeight, maxBounds.y - camHalfHeight);
        }

        float newX = Mathf.SmoothDamp(transform.position.x, desiredX, ref velX, smoothTimeX);
        float newY = Mathf.SmoothDamp(transform.position.y, desiredY, ref velY, smoothTimeY);

        transform.position = new Vector3(newX, newY, transform.position.z);
    }
}
