using BrickVault;
using Diorama.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Rendering
{
    public abstract class RenderBuffer
    {
        public int Handle;

        public long Hash { get; protected set; }

        public abstract ReadOnlySpan<byte> GetBufferSpan();

        public override bool Equals(object? obj)
        {
            if (obj is not RenderBuffer other)
                return false;

            if (GetType() != other.GetType()) return false;

            return GetBufferSpan().SequenceEqual(other.GetBufferSpan());
        }

        public override int GetHashCode()
        {
            return (int)(Hash ^ (Hash >> 32));
        }

        protected abstract long GetChecksum();

        public abstract void Use();

        public abstract void Finalise();
    }
}
