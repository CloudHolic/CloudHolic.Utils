using System.Runtime.InteropServices;

namespace CloudHolic.Utils.Wrappers;

public static class UserWrapper
{
    [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetForegroundWindow(IntPtr hWnd);
}
