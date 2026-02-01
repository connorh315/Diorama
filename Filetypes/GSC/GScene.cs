using Diorama.Filetypes.GSC.Components;
using Diorama.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Filetypes.GSC
{
    public class GScene
    {
        private RawFile file;

        internal Dictionary<int, VertexList> vertexLists = new();
        internal Dictionary<int, ushort[]> indicesLists = new();
        internal int referenceCounter = 5;

        public NuRenderMesh[] RenderMeshes;

        private VertexList GetVertexList()
        {
            int reference = 0;
            int offset = 0;
            uint vertexReference = file.ReadUInt(true);
            if ((vertexReference & 0xc0000000) != 0) {
                // Reference to existing vertex list
                Debug.Assert(1 == 0);
            }
            else
            {
                // New vertex list
                uint unknown = file.ReadUInt(true); // 0x502??
                reference = referenceCounter++;
                VertexList list = VertexList.Parse(file);
                vertexLists.Add(reference, list);
                offset = file.ReadInt(true);
            }

            //VertexListReference listReference = new VertexListReference()
            //{
            //    Reference = reference,
            //    GlobalOffset = offset
            //};

            return vertexLists[reference];
        }

        private ushort[] GetIndexList()
        {
            int reference = 0;
            uint indexReference = file.ReadUInt(true);
            if ((indexReference & 0xc0000000) != 0)
            {
                reference = (int)(indexReference & 0xffff);
                int unknown = file.ReadInt(true);
                return indicesLists[reference];
            }
            else
            {
                Debug.Assert(file.ReadUInt(true) == 0x102); // unknown
                uint indicesCount = file.ReadUInt(true);
                uint unknown3 = file.ReadUInt(true);

                ushort[] indexBuffer = new ushort[indicesCount];
                for (int idx = 0; idx < indicesCount; idx++)
                {
                    indexBuffer[idx] = file.ReadUShort(true);
                }

                reference = referenceCounter++;
                indicesLists.Add(reference, indexBuffer);
            }

            return indicesLists[reference];
        }

        public static void Parse(string filePath)
        {
            GScene gsc = new GScene();

            using (RawFile file = new RawFile(filePath))
            {
                gsc.file = file;

                uint resourceHeaderSize = file.ReadUInt(true);
                file.Seek(resourceHeaderSize, SeekOrigin.Current);

                uint gscSize = file.ReadUInt(true);

                Debug.Assert(file.ReadUInt(true) == 1);

                Debug.Assert(file.ReadString(4) == "02UN");

                uint nu20Version = file.ReadUInt(true);
                Debug.Assert(nu20Version == 0x4f);

                Debug.Assert(file.ReadString(4) == "OFNI");

                uint infoStringCount = file.ReadUInt(true);
                for (int i = 0; i < infoStringCount; i++)
                {
                    string infoString = file.ReadPascalString(true, 256);
                    Console.WriteLine($"Info String {i}: {infoString}");
                }

                Debug.Assert(file.ReadString(4) == "LBTN");

                uint ntblVersion = file.ReadUInt(true);
                Debug.Assert(ntblVersion == 0x4f);

                int ntblLength = file.ReadInt(true);
                string ntblData = file.ReadString(ntblLength);

                Debug.Assert(file.ReadUInt(true) == 0); // defunctTex??

                List<ushort> splines = NuVector.ReadVectorArray<ushort>(file);
                Debug.Assert(splines.Count == 0); // NuSpline structure (replace the ushort)

                List<ushort> vfxLocators = NuVector.ReadVectorArray<ushort>(file);
                Debug.Assert(vfxLocators.Count == 0); // NuVfxLocator structure (replace the ushort)

                Debug.Assert(file.ReadUInt(true) == 1); // possibly number of MESHes?

                Debug.Assert(file.ReadString(4) == "HSEM");

                uint meshVersion = file.ReadUInt(true);
                Debug.Assert(meshVersion == 0xaf); 

                uint partCount = file.ReadUInt(true);
                gsc.RenderMeshes = new NuRenderMesh[partCount];
                for (int part = 0; part < partCount; part++)
                {
                    Debug.Assert(file.ReadUInt(true) == 1); // unknown

                    NuRenderMesh mesh = new NuRenderMesh();

                    uint vertexBufferCount = file.ReadUInt(true);
                    mesh.VertexBuffers = new VertexList[vertexBufferCount];
                    for (int vList = 0; vList < vertexBufferCount; vList++)
                    {
                        gsc.GetVertexList();
                    }

                    uint fastBlendVbsCount = file.ReadUInt(true);
                    for (int fastVList = 0; fastVList < fastBlendVbsCount; fastVList++)
                    {
                        Debug.Assert(1 == 0);
                    }

                    mesh.Indices = gsc.GetIndexList();
                    uint offsetIndices = file.ReadUInt(true);
                    uint indicesCount = file.ReadUInt(true);
                    uint offsetVertices = file.ReadUInt(true);

                    ushort primitiveType = file.ReadUShort(true);
                    Debug.Assert(primitiveType == 0, "Unknown primitive type");

                    uint verticesCount = file.ReadUInt(true);

                    int vbUsedCount = file.ReadInt(true);
                    Debug.Assert(vbUsedCount == 0);

                    int vbInstBitsAsU32 = file.ReadInt(true);
                    Debug.Assert(vbInstBitsAsU32 == 0);

                    uint skinMtxMap = file.ReadUInt(true);
                    Debug.Assert(skinMtxMap == 0); // legacy array?

                    uint defunctOptFlags = file.ReadUInt(true);
                    Debug.Assert(defunctOptFlags == 0);

                    Vector4 centreExtents0 = new Vector4(file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true));
                    Vector4 centreExtents1 = new Vector4(file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true));

                    uint densityDiscDiameter = file.ReadUInt(true);

                    gsc.referenceCounter += 2;

                    Console.WriteLine();
                }
            }
        }
    }
}
