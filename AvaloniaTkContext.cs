using Avalonia.OpenGL;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama
{
    internal class AvaloniaTkContext : IBindingsContext
    {
        private readonly GlInterface _glInterface;

        public AvaloniaTkContext(GlInterface glInterface)
        {
            _glInterface = glInterface;
        }

        public IntPtr GetProcAddress(string procName) => _glInterface.GetProcAddress(procName);
    }
}
