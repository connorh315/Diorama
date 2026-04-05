using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Extensions
{
    public static class MathExtensions
    {
        public static System.Numerics.Vector2 ToNumerics(this OpenTK.Mathematics.Vector2 v) => new System.Numerics.Vector2(v.X, v.Y);
        public static OpenTK.Mathematics.Vector2 ToOpenTK(this System.Numerics.Vector2 v) => new OpenTK.Mathematics.Vector2(v.X, v.Y);

        public static System.Numerics.Vector3 ToNumerics(this OpenTK.Mathematics.Vector3 v) => new System.Numerics.Vector3(v.X, v.Y, v.Z);
        public static OpenTK.Mathematics.Vector3 ToOpenTK(this System.Numerics.Vector3 v) => new OpenTK.Mathematics.Vector3(v.X, v.Y, v.Z);

        public static System.Numerics.Vector4 ToNumerics(this OpenTK.Mathematics.Vector4 v) => new System.Numerics.Vector4(v.X, v.Y, v.Z, v.W);
        public static OpenTK.Mathematics.Vector4 ToOpenTK(this System.Numerics.Vector4 v) => new OpenTK.Mathematics.Vector4(v.X, v.Y, v.Z, v.W);
    }
}
