using UnityEngine;

public class AlignVisualToColliderBottom : MonoBehaviour
{
    [SerializeField] private CapsuleCollider2D boxCollider;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform visual;

    [Header("Ajuste fino")]
    [SerializeField] private float yOffset = 0f;

    private void Awake()
    {
        if (boxCollider == null)
            boxCollider = GetComponent<CapsuleCollider2D>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (visual == null && spriteRenderer != null)
            visual = spriteRenderer.transform;
    }

    private void LateUpdate()
    {
        AlignBottom();
    }

    private void AlignBottom()
    {
        if (boxCollider == null || spriteRenderer == null || visual == null)
            return;

        float colliderBottom = boxCollider.bounds.min.y;
        float spriteBottom = spriteRenderer.bounds.min.y;

        float difference = colliderBottom - spriteBottom + yOffset;

        visual.position += new Vector3(0f, difference, 0f);
    }
}