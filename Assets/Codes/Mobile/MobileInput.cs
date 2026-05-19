public static class MobileInput
{
    public static bool LeftHeld;
    public static bool RightHeld;
    public static bool JumpPressed;
    public static bool JumpHeld;
    public static bool JumpReleased;
    public static bool DashPressed;
    public static bool CrouchHeld;
    public static bool RestartPressed;

    public static float HorizontalAxis => RightHeld ? 1f : (LeftHeld ? -1f : 0f);

    public static void Reset()
    {
        LeftHeld = false;
        RightHeld = false;
        JumpPressed = false;
        JumpHeld = false;
        JumpReleased = false;
        DashPressed = false;
        CrouchHeld = false;
        RestartPressed = false;
    }
}
