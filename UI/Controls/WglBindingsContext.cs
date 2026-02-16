using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Runtime.InteropServices;

namespace Diorama.UI.Controls
{


    public class WglBindingsContext : IBindingsContext
    {
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        public IntPtr GetProcAddress(string procName)
        {
            var ptr = Win32.wglGetProcAddress(procName);

            if (ptr == IntPtr.Zero)
            {
                var module = GetModuleHandle("opengl32.dll");
                ptr = GetProcAddress(module, procName);
            }

            return ptr;
        }
    }
}
