using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core
{
    public static class VectorExtensions
    {
        public static Vector2 ToVector2(this Vector4 v)
            => new Vector2(v.Z, v.W);

        public static Vector3 ToVector3(this Vector4 v)
            => new Vector3(v.X, v.Y, v.Z);
    }

}
