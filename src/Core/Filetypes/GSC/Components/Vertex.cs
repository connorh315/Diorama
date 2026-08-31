using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class Vertex
    {
        public Vector3 Position;

        public Vector3 Normal;

        public Vector3 Tangent;

        public Vector4 ColorSet0;

        public Vector4 ColorSet1;

        public Vector4 UVSet01;

        public Vector4 UVSet23;

        public VectorI4 BlendIndices;

        public Vector4 BlendWeights;
    }

    public struct VectorI4
    {
        public ushort X;
        public ushort Y;
        public ushort Z;
        public ushort W;

        public VectorI4(ushort x, ushort y, ushort z, ushort w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }
    }
}
