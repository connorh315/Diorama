using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Diorama.UI.Controls;

internal static class Win32
{
    const int WS_CHILD = 0x40000000;
    const int WS_VISIBLE = 0x10000000;
    const int WS_CLIPSIBLINGS = 0x04000000;
    const int WS_DISABLED = 0x08000000;

    const uint CS_VREDRAW = 0x0001;
    const uint CS_HREDRAW = 0x0002;
    const uint CS_OWNDC = 0x0020;

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

    private static string GetClassName(string cursorType)
    {
        return "OpenGLHostWindow" + (cursorType == "Hand" ? "Hand" : "");
    }

    public static IntPtr CreateChildWindow(IntPtr parent, string cursor)
    {
        RegisterGlWindowClass(cursor);

        return CreateWindowEx(
            0, GetClassName(cursor), "",
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

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct WNDCLASSEX
    {
        public uint cbSize;
        public uint style;
        public IntPtr lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public IntPtr hInstance;
        public IntPtr hIcon;
        public IntPtr hCursor;
        public IntPtr hbrBackground;
        public string lpszMenuName;
        public string lpszClassName;
        public IntPtr hIconSm;
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern ushort RegisterClassEx(ref WNDCLASSEX lpwcx);

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

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsWindowVisible(IntPtr hWnd);

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

    private static readonly WndProcDelegate DefaultWndProc = DefWindowProc;

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr DefWindowProc(
        IntPtr hWnd,
        int msg,
        IntPtr wParam,
        IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern IntPtr LoadCursor(
    IntPtr hInstance,
    IntPtr lpCursorName);

    public static readonly IntPtr IDC_ARROW = (IntPtr)32512;
    public static readonly IntPtr IDC_HAND = (IntPtr)32649;

    private static bool registeredHand;
    private static bool registeredArrow;

    public static void RegisterGlWindowClass(string cursorType)
    {
        IntPtr cursor;
        if (cursorType == "Hand")
        {
            if (registeredHand) return;
            cursor = LoadCursor(IntPtr.Zero, IDC_HAND);
            registeredHand = true;
        }
        else
        {
            if (registeredArrow) return;
            cursor = LoadCursor(IntPtr.Zero, IDC_ARROW);
            registeredArrow = true;
        }

        string className = GetClassName(cursorType);

        WNDCLASSEX wc = new()
        {
            cbSize = (uint)Marshal.SizeOf<WNDCLASSEX>(),
            style = CS_OWNDC | CS_HREDRAW | CS_VREDRAW,

            lpfnWndProc = Marshal.GetFunctionPointerForDelegate(DefaultWndProc),

            hInstance = GetModuleHandle(null),

            hCursor = cursor,

            // THIS is the important part
            hbrBackground = IntPtr.Zero,

            lpszClassName = className
        };

        ushort atom = RegisterClassEx(ref wc);

        if (atom == 0)
            throw new Win32Exception(Marshal.GetLastWin32Error());
    }

    private const int GWL_STYLE = -16;

    private const int WS_MINIMIZEBOX = 0x00020000;
    private const int WS_MAXIMIZEBOX = 0x00010000;

    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_NOSIZE = 0x0001;
    private const uint SWP_NOZORDER = 0x0004;
    private const uint SWP_FRAMECHANGED = 0x0020;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(
    nint hWnd,
    nint hWndInsertAfter,
    int X,
    int Y,
    int cx,
    int cy,
    uint uFlags);

    public static void RemoveMinMaxButtons(nint hwnd)
    {
        var style = GetWindowLongPtr(hwnd, GWL_STYLE).ToInt64();

        style &= ~WS_MINIMIZEBOX;
        style &= ~WS_MAXIMIZEBOX;

        SetWindowLongPtr(hwnd, GWL_STYLE, (nint)style);

        SetWindowPos(
            hwnd,
            nint.Zero,
            0, 0, 0, 0,
            SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
    }
}
