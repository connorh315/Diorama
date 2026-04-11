using Diorama.Core;
using Diorama.Core.Filetypes.GSC.Components;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Rendering
{
    public class RenderVertexBuffer : RenderBuffer
    {
        public int Stride;
        public VertexDefinition[] Attributes;

        public VertexList Original;

        public byte[] Buffer => Original.VerticesDump;

        public override ReadOnlySpan<byte> GetBufferSpan()
        {
            return Buffer;
        }

        public override void Use()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, Handle);
        }

        protected override long GetChecksum()
        {
            long checksum = NuExtensions.CRC_FNV_OFFSET_64;

            for (int i = 0; i < Buffer.Length; i++)
            {
                checksum ^= Buffer[i];
                checksum *= NuExtensions.CRC_FNV_PRIME_64;
            }

            return checksum;
        }

        public override void Finalise()
        {
            Handle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, Handle);
            GL.BufferData(
                BufferTarget.ArrayBuffer,
                Buffer.Length,
                Buffer,
                BufferUsageHint.StaticDraw);
        }

        public override bool Equals(object? obj)
        {
            if (obj is not RenderVertexBuffer other) return false;

            if (Attributes.Length != other.Attributes.Length) return false;

            if (Stride != other.Stride) return false;

            for (int i = 0; i < Attributes.Length; i++)
            {
                if (Attributes[i].Variable != other.Attributes[i].Variable) return false;
            }

            return base.Equals(obj);
        }

        public static RenderVertexBuffer FromBuffer(VertexList buffer)
        {
            RenderVertexBuffer vertexBuffer = new RenderVertexBuffer();

            vertexBuffer.Original = buffer;

            vertexBuffer.Stride = buffer.Stride;

            vertexBuffer.Attributes = buffer.Definitions;

            vertexBuffer.Hash = vertexBuffer.GetHashCode();

            return vertexBuffer;
        }
    }
}
