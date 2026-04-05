using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuRenderMesh
    {
        public VertexList[] VertexBuffers;
        public int[] VertexBufferOffsets;
        public ushort[] Indices;

        public uint VerticesBase;
        public uint VerticesCount;
        public uint IndicesBase;
        public uint IndicesCount;

        public uint VbInstBits;

        public Vector4[] CentreExtents = new Vector4[2];

        public float DensityDiscDiameter;
    }
}
