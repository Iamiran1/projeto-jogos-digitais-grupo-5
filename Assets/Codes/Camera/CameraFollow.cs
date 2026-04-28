using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Alvo")]
    public Transform target;

    [Header("Seguimento")]
    public Vector2 offset = new Vector2(0f, 1f);

    [Header("Limites do Mapa")]
    public bool useBounds = false;
    public Vector2 minBounds;
    public Vector2 maxBounds;

    private float camHalfHeight;
    private float camHalfWidth;
    private float standingHalfHeight;

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
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Compensate for crouch: when localScale.y < 1 the center moves down,
        // so we shift the target Y back up to keep the camera at standing height.
        float scaleY   = target.localScale.y;
        float targetY  = target.position.y + standingHalfHeight * (1f - scaleY);

        Vector3 desired = new Vector3(
            target.position.x + offset.x,
            targetY + offset.y,
            transform.position.z
        );

        if (useBounds)
        {
            desired.x = Mathf.Clamp(desired.x, minBounds.x + camHalfWidth,  maxBounds.x - camHalfWidth);
            desired.y = Mathf.Clamp(desired.y, minBounds.y + camHalfHeight, maxBounds.y - camHalfHeight);
        }

        transform.position = desired;
    }
}
