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

    void Start()
    {
        var cam = GetComponent<Camera>();
        camHalfHeight = cam.orthographicSize;
        camHalfWidth  = camHalfHeight * cam.aspect;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desired = new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y,
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
