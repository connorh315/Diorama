using Diorama.Filetypes.GSC.Components;
using Diorama.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Filetypes.GSC
{
    public class GScene_4F : GScene
    {
        protected override VertexList GetVertexList()
        {
            int reference = 0;
            int offset = 0;
            uint vertexReference = file.ReadUInt(true);
            if ((vertexReference & 0xc0000000) != 0)
            {
                // Reference to existing vertex list
                reference = (int)(vertexReference & 0xffff);
                uint unk = file.ReadUInt(true);
                offset = file.ReadInt(true);
            }
            else
            {
                // New vertex list
                uint unknown = file.ReadUInt(true); // 0x502??
                reference = referenceCounter++;
                VertexList list = VertexList.Parse(file);
                vertexLists.Add(reference, list);
                offset = file.ReadInt(true);
                Debug.Assert(offset == 0);
            }

            return vertexLists[reference];
        }

        protected override ushort[] GetIndexList()
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
                file.ReadUInt(true); // unknown
                uint indicesCount = file.ReadUInt(true);
                uint unknown3 = file.ReadUInt(true);

                ushort[] indexBuffer = new ushort[indicesCount];
                for (int idx = 0; idx < indicesCount; idx++)
                {
                    indexBuffer[idx] = file.ReadUShort(false);
                }

                reference = referenceCounter++;
                indicesLists.Add(reference, indexBuffer);
            }

            return indicesLists[reference];
        }

        protected virtual void ReadInfo()
        {
            Debug.Assert(file.ReadString(4) == "OFNI");

            uint infoStringCount = file.ReadUInt(true);
            for (int i = 0; i < infoStringCount; i++)
            {
                string infoString = file.ReadPascalString(true, 256);
                //Console.WriteLine($"Info String {i}: {infoString}");
            }
        }

        protected virtual void ReadNameTable()
        {
            NuNameTable nameTable = NuNameTable.Read(file);
        }

        protected virtual void ReadSplines()
        {
            List<NuSpline> splines = NuVector.ReadVectorArray<NuSpline>(file);
        }

        protected override void Parse()
        {
            ReadInfo();

            ReadNameTable();

            uint defunctHasSeperateTextureFiles = file.ReadUInt(true);
            if (defunctHasSeperateTextureFiles != 0)
            {
                referenceCounter++;
                TextureHeaders headers = TextureHeaders.Read(file);
                Debug.Assert(defunctHasSeperateTextureFiles == 1);
            }

            ReadSplines();
            //Debug.Assert(splines.Count == 0); // NuSpline structure (replace the ushort)

            List<NuVfxLocator> vfxLocators = NuVector.ReadVectorArray<NuVfxLocator>(file);

            Debug.Assert(file.ReadUInt(true) == 1); // possibly number of MESHes?

            Debug.Assert(file.ReadString(4) == "HSEM");

            uint meshVersion = file.ReadUInt(true);
            Debug.Assert(meshVersion == 0xaf);

            uint partCount = file.ReadUInt(true);
            RenderMeshes = new NuRenderMesh[partCount];
            for (int part = 0; part < partCount; part++)
            {
                Debug.Assert(file.ReadUInt(true) == 1); // unknown

                NuRenderMesh mesh = new NuRenderMesh();

                uint vertexBufferCount = file.ReadUInt(true);
                mesh.VertexBuffers = new VertexList[vertexBufferCount];
                for (int vList = 0; vList < vertexBufferCount; vList++)
                {
                    mesh.VertexBuffers[vList] = GetVertexList();
                }

                uint fastBlendVbsCount = file.ReadUInt(true);
                for (int fastVList = 0; fastVList < fastBlendVbsCount; fastVList++)
                {
                    GetVertexList();
                }
                if (fastBlendVbsCount != 0)
                {
                    referenceCounter += 1;
                }

                mesh.Indices = GetIndexList();
                mesh.IndicesBase = file.ReadUInt(true);
                mesh.IndicesCount = file.ReadUInt(true);
                mesh.VerticesBase = file.ReadUInt(true);

                ushort primitiveType = file.ReadUShort(true);
                Debug.Assert(primitiveType == 0, "Unknown primitive type");

                mesh.VerticesCount = file.ReadUInt(true);

                int vbUsedCount = file.ReadInt(true);
                //Debug.Assert(vbUsedCount == 0);

                int vbInstBitsAsU32 = file.ReadInt(true);
                Debug.Assert(vbInstBitsAsU32 == 0);

                uint skinMtxMap = file.ReadUInt(true);
                Debug.Assert(skinMtxMap == 0); // legacy array?

                uint defunctOptFlags = file.ReadUInt(true);
                Debug.Assert(defunctOptFlags == 0);

                Vector4 centreExtents0 = new Vector4(file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true));
                Vector4 centreExtents1 = new Vector4(file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true));

                float densityDiscDiameter = file.ReadFloat(true);

                referenceCounter += 2;

                RenderMeshes[part] = mesh;
            }
        }
    }
}
