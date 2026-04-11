using Diorama.Core;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Rendering
{
    public class RenderIndicesBuffer : RenderBuffer
    {
        public int Count;

        public ushort[] Indices;

        public override ReadOnlySpan<byte> GetBufferSpan()
        {
            return MemoryMarshal.AsBytes(Indices.AsSpan());
        }

        public override void Use()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, Handle);
        }

        public override bool Equals(object? obj)
        {
            if (obj is not RenderIndicesBuffer other) return false;

            return Indices.SequenceEqual(other.Indices);
        }

        protected override long GetChecksum()
        {
            long checksum = NuExtensions.CRC_FNV_OFFSET_64;
            for (int i = 0; i < Indices.Length; i++)
            {
                ushort v = Indices[i];

                checksum ^= (byte)(v & 0xff);
                checksum *= NuExtensions.CRC_FNV_PRIME_64;

                checksum ^= (byte)(v >> 8);
                checksum *= NuExtensions.CRC_FNV_PRIME_64;
            }

            return checksum;
        }

        public static RenderIndicesBuffer FromBuffer(ushort[] buffer)
        {
            RenderIndicesBuffer indices = new RenderIndicesBuffer();
            
            indices.Handle = GL.GenBuffer();
            indices.Count = buffer.Length;
            indices.Indices = buffer;

            indices.Hash = indices.GetChecksum();

            return indices;
        }

        public override void Finalise()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, Handle);
            GL.BufferData(
                BufferTarget.ElementArrayBuffer,
                Indices.Length * sizeof(ushort),
                Indices,
                BufferUsageHint.StaticDraw);
        }
    }
}
