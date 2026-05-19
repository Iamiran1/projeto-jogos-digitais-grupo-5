using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform knob;

    private float radius;
    private int activePointerId = -1;

    private const float HorizontalDeadzone = 0.25f;
    private const float DownDeadzone = 0.40f;

    void Start()
    {
        radius = GetComponent<RectTransform>().rect.width * 0.5f;
        if (knob != null) knob.GetComponent<UnityEngine.UI.Image>().raycastTarget = false;
    }

    public void OnPointerDown(PointerEventData data)
    {
        if (activePointerId != -1) return;
        activePointerId = data.pointerId;
        UpdateKnob(data);
    }

    public void OnDrag(PointerEventData data)
    {
        if (data.pointerId != activePointerId) return;
        UpdateKnob(data);
    }

    public void OnPointerUp(PointerEventData data)
    {
        if (data.pointerId != activePointerId) return;
        activePointerId = -1;
        if (knob != null) knob.localPosition = Vector2.zero;
        MobileInput.LeftHeld = false;
        MobileInput.RightHeld = false;
        MobileInput.CrouchHeld = false;
    }

    private void UpdateKnob(PointerEventData data)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            GetComponent<RectTransform>(),
            data.position,
            data.pressEventCamera,
            out Vector2 localPoint);

        Vector2 clamped = Vector2.ClampMagnitude(localPoint, radius);
        if (knob != null) knob.localPosition = clamped;

        Vector2 normalized = clamped / radius;
        MobileInput.LeftHeld  = normalized.x < -HorizontalDeadzone;
        MobileInput.RightHeld = normalized.x >  HorizontalDeadzone;
        MobileInput.CrouchHeld = normalized.y < -DownDeadzone;
    }
}
