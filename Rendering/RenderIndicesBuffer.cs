using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Rendering
{
    public class RenderIndicesBuffer
    {
        public int Handle;
        public int Count;

        public void Use()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, Handle);
        }

        public static RenderIndicesBuffer FromBuffer(ushort[] buffer)
        {
            RenderIndicesBuffer indices = new RenderIndicesBuffer();
            
            indices.Handle = GL.GenBuffer();
            indices.Count = buffer.Length;

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indices.Handle);
            GL.BufferData(
                BufferTarget.ElementArrayBuffer,
                buffer.Length * sizeof(ushort),
                buffer,
                BufferUsageHint.StaticDraw);

            return indices;
        }
    }
}
