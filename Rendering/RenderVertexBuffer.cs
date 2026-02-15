using Diorama.Core.Filetypes.GSC.Components;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Rendering
{
    public class RenderVertexBuffer
    {
        public int Handle;
        public int Stride;
        public VertexDefinition[] Attributes;

        public void Use()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, Handle);
        }

        public static RenderVertexBuffer FromBuffer(VertexList buffer)
        {
            RenderVertexBuffer vertexBuffer = new RenderVertexBuffer();

            vertexBuffer.Stride = buffer.Stride;

            vertexBuffer.Attributes = buffer.Definitions;

            vertexBuffer.Handle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer.Handle);
            GL.BufferData(
                BufferTarget.ArrayBuffer,
                buffer.VerticesDump.Length,
                buffer.VerticesDump,
                BufferUsageHint.StaticDraw);

            return vertexBuffer;
        }
    }
}
