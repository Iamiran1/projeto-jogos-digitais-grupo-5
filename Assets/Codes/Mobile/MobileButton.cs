using UnityEngine;
using UnityEngine.EventSystems;

public enum MobileButtonAction
{
    Left, Right, Jump, Dash, Crouch,
    Restart, Pause, Resume, BackToMenu
}

[RequireComponent(typeof(UnityEngine.UI.Image))]
public class MobileButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] private MobileButtonAction action;

    public void OnPointerDown(PointerEventData data)
    {
        switch (action)
        {
            case MobileButtonAction.Left:       MobileInput.LeftHeld = true; break;
            case MobileButtonAction.Right:      MobileInput.RightHeld = true; break;
            case MobileButtonAction.Jump:       MobileInput.JumpPressed = true; MobileInput.JumpHeld = true; break;
            case MobileButtonAction.Dash:       MobileInput.DashPressed = true; break;
            case MobileButtonAction.Crouch:     MobileInput.CrouchHeld = true; break;
            case MobileButtonAction.Restart:    MobileInput.RestartPressed = true; break;
            case MobileButtonAction.Pause:      MobileHUD.Instance?.TogglePause(); break;
            case MobileButtonAction.Resume:     MobileHUD.Instance?.Resume(); break;
            case MobileButtonAction.BackToMenu: MobileHUD.Instance?.BackToMenu(); break;
        }
    }

    public void OnPointerUp(PointerEventData data) => Release();
    public void OnPointerExit(PointerEventData data) => Release();

    private void Release()
    {
        switch (action)
        {
            case MobileButtonAction.Left:   MobileInput.LeftHeld = false; break;
            case MobileButtonAction.Right:  MobileInput.RightHeld = false; break;
            case MobileButtonAction.Jump:   MobileInput.JumpReleased = true; MobileInput.JumpHeld = false; break;
            case MobileButtonAction.Crouch: MobileInput.CrouchHeld = false; break;
        }
    }
}
