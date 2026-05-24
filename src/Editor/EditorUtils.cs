using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using System.Text;

namespace Diorama.Editor
{
    public static class EditorUtils
    {
        private static readonly Matrix4 Flip = Matrix4.CreateScale(1f, 1f, -1f);
        public static Vector3 FlipCoordSpace(Vector3 pos)
        {
            return (Flip * new Vector4(pos, 1f)).Xyz;
        }
    }
}
