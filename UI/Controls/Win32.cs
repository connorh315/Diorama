using System;
using System.Runtime.InteropServices;

namespace Diorama.UI.Controls;

internal static class Win32
{
    const int WS_CHILD = 0x40000000;
    const int WS_VISIBLE = 0x10000000;
    const int WS_CLIPSIBLINGS = 0x04000000;
    const int WS_DISABLED = 0x08000000;

    [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
    private static extern int GetWindowLong32(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
    private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

    public static IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex)
    {
        return IntPtr.Size == 8
            ? GetWindowLongPtr64(hWnd, nIndex)
            : (IntPtr)GetWindowLong32(hWnd, nIndex);
    }

    [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
    private static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
    private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    public static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
    {
        return IntPtr.Size == 8
            ? SetWindowLongPtr64(hWnd, nIndex, dwNewLong)
            : (IntPtr)SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32());
    }

    public static IntPtr CreateChildWindow(IntPtr parent)
    {
        return CreateWindowEx(
            0, "STATIC", "",
            WS_CHILD | WS_VISIBLE,
            0, 0, 100, 100,
            parent, IntPtr.Zero, GetModuleHandle(null), IntPtr.Zero);
    }

    [DllImport("kernel32.dll")]
    public static extern IntPtr GetModuleHandle(string? lpModuleName);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr CreateWindowEx(
        int exStyle,
        string className,
        string windowName,
        int style,
        int x,
        int y,
        int width,
        int height,
        IntPtr parent,
        IntPtr menu,
        IntPtr instance,
        IntPtr param);

    [DllImport("user32.dll")]
    public static extern bool DestroyWindow(IntPtr hwnd);

    [DllImport("user32.dll")]
    public static extern IntPtr GetDC(IntPtr hwnd);

    [DllImport("user32.dll")]
    public static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

    [DllImport("gdi32.dll")]
    public static extern bool SwapBuffers(IntPtr hdc);

    [DllImport("opengl32.dll")]
    public static extern IntPtr wglCreateContext(IntPtr hdc);

    [DllImport("opengl32.dll")]
    public static extern bool wglMakeCurrent(IntPtr hdc, IntPtr context);

    [DllImport("opengl32.dll")]
    public static extern bool wglDeleteContext(IntPtr context);

    [DllImport("opengl32.dll")]
    public static extern IntPtr wglGetProcAddress(string name);

    public delegate IntPtr WndProcDelegate(
    IntPtr hWnd,
    int msg,
    IntPtr wParam,
    IntPtr lParam);

    public delegate IntPtr wglCreateContextAttribsARBProc(
        IntPtr hdc,
        IntPtr shareContext,
        int[] attribList);

    public const int GWL_WNDPROC = -4;

    //[DllImport("user32.dll", SetLastError = true)]
    //public static extern IntPtr SetWindowLongPtr(
    //    IntPtr hWnd,
    //    int nIndex,
    //    IntPtr dwNewLong);

    [DllImport("user32.dll")]
    public static extern IntPtr CallWindowProc(
        IntPtr lpPrevWndFunc,
        IntPtr hWnd,
        int msg,
        IntPtr wParam,
        IntPtr lParam);

    public const int RGN_AND = 1;
    [DllImport("user32.dll", SetLastError = true)]
    public static extern int SetWindowRgn(
    IntPtr hWnd,
    IntPtr hRgn,
    bool bRedraw);

    [DllImport("gdi32.dll", SetLastError = true)]
    public static extern IntPtr CreateRectRgn(
    int left,
    int top,
    int right,
    int bottom);

    public static T wglGetProcAddressDelegate<T>(string name) where T : Delegate
    {
        var ptr = wglGetProcAddress(name);
        return Marshal.GetDelegateForFunctionPointer<T>(ptr);
    }

    public static void SetPixelFormat(IntPtr hdc)
    {
        PIXELFORMATDESCRIPTOR pfd = new PIXELFORMATDESCRIPTOR
        {
            nSize = (ushort)Marshal.SizeOf<PIXELFORMATDESCRIPTOR>(),
            nVersion = 1,
            dwFlags = 0x00000004 | 0x00000020 | 0x00000001,
            iPixelType = 0,
            cColorBits = 32,
            cDepthBits = 24,
            cStencilBits = 8
        };

        int format = ChoosePixelFormat(hdc, ref pfd);
        SetPixelFormat(hdc, format, ref pfd);
    }

    [DllImport("gdi32.dll")]
    static extern int ChoosePixelFormat(IntPtr hdc, ref PIXELFORMATDESCRIPTOR pfd);

    [DllImport("gdi32.dll")]
    static extern bool SetPixelFormat(IntPtr hdc, int format, ref PIXELFORMATDESCRIPTOR pfd);

    [StructLayout(LayoutKind.Sequential)]
    struct PIXELFORMATDESCRIPTOR
    {
        public ushort nSize;
        public ushort nVersion;
        public uint dwFlags;
        public byte iPixelType;
        public byte cColorBits;
        public byte cRedBits, cRedShift;
        public byte cGreenBits, cGreenShift;
        public byte cBlueBits, cBlueShift;
        public byte cAlphaBits, cAlphaShift;
        public byte cAccumBits;
        public byte cAccumRedBits, cAccumGreenBits, cAccumBlueBits, cAccumAlphaBits;
        public byte cDepthBits;
        public byte cStencilBits;
        public byte cAuxBuffers;
        public byte iLayerType;
        public byte bReserved;
        public uint dwLayerMask;
        public uint dwVisibleMask;
        public uint dwDamageMask;
    }

    public const int GWL_EXSTYLE = -20;
    public const int WS_EX_NOACTIVATE = 0x08000000;

    [DllImport("user32.dll")]
    public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
}
