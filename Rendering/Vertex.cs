using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Mathematics;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Rendering
{
    public struct Vertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector4 UVSet01;
        public Color4 Color;

        public static int Size => 12 + 12 + 16 + 16;

        public Vertex(Vector3 pos, Vector3 normal, Vector4 uv, Color4 color)
        {
            Position = pos; Normal = normal; UVSet01 = uv; Color = color;
        }
    }
}
