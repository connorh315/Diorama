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
        public uint[] VertexBufferFlags;
        public int[] VertexBufferOffsets;
        public VertexList[] FastBlendVBs;
        public uint[] FastBlendFlags;
        public int[] FastBlendOffsets;
        public ushort[] Indices;
        public uint IndicesFlags; // 0x102 normally, becomes 0x103 on grass

        public uint VerticesBase;
        public uint VerticesCount;
        public uint IndicesBase;
        public uint IndicesCount;

        public uint VbInstBits;

        public List<byte> SkinMtxMap;

        public NuBlendShape Shape;

        public uint DefunctOptFlags;

        public Vector4[] CentreExtents = new Vector4[2];

        public float DensityDiscDiameter;

        public uint DepthBits;

        public VertexList DepthVertexBuffer;
        public uint DepthVbUsedCount;
        public ushort[] DepthIndices;
        public uint DepthIndicesFlags;
        public uint DepthVerticesBase;
        public uint DepthVerticesCount;
        public uint DepthFlags;
        public int DepthOffsets;
        public uint DepthIndicesBase;
        public uint DepthIndicesOffset;
        public uint DepthIndicesBase2;

    }
}
