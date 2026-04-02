using BrickVault;
using Diorama.Core.Types;
using System.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuMeshSceneBlock
    {
        public uint Version { get; set; }

        public NuRenderMesh[] Meshes { get; set; }

        protected VertexList GetVertexList(RawFile file, GSerializationContext ctx, ref int offset)
        {
            int reference = 0;
            uint vertexReference = file.ReadUInt(true);
            if ((vertexReference & 0xc0000000) != 0)
            {
                // Reference to existing vertex list
                reference = (int)(vertexReference & 0xffff);
                uint one = file.ReadUInt(true);
                offset = file.ReadInt(true);
                return ctx.GetObject<VertexList>(reference);
            }
            else
            {
                // New vertex list
                uint flags = file.ReadUInt(true); // 0x502??
                VertexList list = VertexList.Parse(file);
                ctx.AddReference(list);
                offset = file.ReadInt(true);
                Debug.Assert(offset == 0);
                return list;
            }
        }

        protected ushort[] GetIndexList(RawFile file, GSerializationContext ctx)
        {
            int reference = 0;
            uint indexReference = file.ReadUInt(true);
            if ((indexReference & 0xc0000000) != 0)
            {
                reference = (int)(indexReference & 0xffff);
                int unknown = file.ReadInt(true);
                return ctx.GetObject<ushort[]>(reference);
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

                ctx.AddReference(indexBuffer);
                return indexBuffer;
            }
        }

        public static NuMeshSceneBlock Parse(RawFile file, GSerializationContext ctx)
        {
            Debug.Assert(file.ReadString(4) == "HSEM");

            NuMeshSceneBlock meshBlock = new NuMeshSceneBlock();
            meshBlock.Version = file.ReadUInt(true);
            Debug.Assert(meshBlock.Version == 0xaf);

            meshBlock.Meshes = new NuRenderMesh[file.ReadInt(true)];
            for (int i = 0; i < meshBlock.Meshes.Length; i++)
            {
                Debug.Assert(file.ReadUInt(true) == 0x1);

                NuRenderMesh mesh = new NuRenderMesh();

                uint vertexBufferCount = file.ReadUInt(true);
                mesh.VertexBuffers = new VertexList[vertexBufferCount];
                mesh.VertexBufferOffsets = new int[vertexBufferCount];

                for (int vList = 0; vList < vertexBufferCount; vList++)
                {
                    mesh.VertexBuffers[vList] = meshBlock.GetVertexList(file, ctx, ref mesh.VertexBufferOffsets[vList]); // Not really sure what the "offset" field is / what it's for
                }

                uint fastBlendVbsCount = file.ReadUInt(true);
                VertexList[] fastBlendVbs = new VertexList[fastBlendVbsCount];
                for (int fastVList = 0; fastVList < fastBlendVbsCount; fastVList++)
                {
                    int offset = 0;
                    fastBlendVbs[fastVList] = meshBlock.GetVertexList(file, ctx, ref offset); // TODO: Properly implement VertexBufferOffsets
                }
                if (fastBlendVbsCount != 0)
                {
                    ctx.AddReference(fastBlendVbs);
                }

                mesh.Indices = meshBlock.GetIndexList(file, ctx);
                mesh.IndicesBase = file.ReadUInt(true);
                mesh.IndicesCount = file.ReadUInt(true);
                mesh.VerticesBase = file.ReadUInt(true);

                ushort primitiveType = file.ReadUShort(true);
                Debug.Assert(primitiveType == 0, "Unknown primitive type");

                mesh.VerticesCount = file.ReadUInt(true);

                mesh.VbInstBits = file.ReadUInt(true);

                List<byte> skinMtxMap = NuSerializer.ReadLegacyVarArray<byte>(file);
                if (skinMtxMap.Count != 0)
                {
                    ctx.AddReference(skinMtxMap);
                }
                //Debug.Assert(skinMtxMap.Count == 0); // legacy array?

                int nuBlendShape = file.ReadInt(true); // i think
                if (nuBlendShape != 0)
                {
                    NuBlendShape.Parse(file, ctx, meshBlock.Version);
                }
                //Debug.Assert(defunctOptFlags == 0);

                uint defunctOptFlags = file.ReadUInt(true);

                for (int j = 0; j < 2; j++)
                {
                    mesh.CentreExtents[j] = new Vector4(file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true));
                }
                ctx.AddReference(mesh.CentreExtents);

                mesh.DensityDiscDiameter = file.ReadFloat(true);

                ctx.AddReference(mesh);

                meshBlock.Meshes[i] = mesh;
            }

            return meshBlock;
        }

        public void Write(RawFile file, GSerializationContext ctx)
        {
            file.WriteString("HSEM");

            file.WriteUInt(Version, true);

            file.WriteInt(Meshes.Length, true);
            for (int i = 0; i < Meshes.Length; i++)
            {
                file.WriteInt(1, true);

                var mesh = Meshes[i];

                file.WriteInt(mesh.VertexBuffers.Length, true);
                for (int vList = 0; vList < mesh.VertexBuffers.Length; vList++)
                {
                    var buffer = mesh.VertexBuffers[vList];
                    if (ctx.GetOrAddReference(buffer, out int reference))
                    {
                        file.WriteUInt((uint)(reference | 0xc0000000), true);
                        file.WriteUInt(1, true);
                        file.WriteInt(mesh.VertexBufferOffsets[vList], true); // Could really do with finding out what this actually is
                    }
                    else
                    {
                        file.WriteUInt((uint)1, true);
                        file.WriteUInt(0x502, true);
                        buffer.Write(file);
                        file.WriteInt(mesh.VertexBufferOffsets[vList], true);
                    }
                }

                file.WriteInt(0, true); // TODO: Fast blend VBs

                if (ctx.GetOrAddReference(mesh.Indices, out int indReference))
                {
                    file.WriteUInt((uint)(indReference | 0xc0000000), true);
                    file.WriteInt(1, true);
                }
                else
                {
                    file.WriteInt(1, true);
                    file.WriteInt(0x102, true);
                    file.WriteInt(mesh.Indices.Length, true);
                    file.WriteInt(2, true);

                    for (int idx = 0; idx < mesh.Indices.Length; idx++)
                    {
                        file.WriteUShort(mesh.Indices[idx], false);
                    }
                }

                file.WriteUInt(mesh.IndicesBase, true);
                file.WriteUInt(mesh.IndicesCount, true);
                file.WriteUInt(mesh.VerticesBase, true);

                file.WriteShort(0, true); // primitive type

                file.WriteUInt(mesh.VerticesCount, true);

                file.WriteUInt(mesh.VbInstBits, true);

                file.WriteInt(0, true); // skin mtx map legacy array

                file.WriteInt(0, true); // nu blend shape

                file.WriteInt(0, true); // defunct opt flags

                for (int j = 0; j < 2; j++)
                {
                    file.WriteFloat(mesh.CentreExtents[j].X, true);
                    file.WriteFloat(mesh.CentreExtents[j].Y, true);
                    file.WriteFloat(mesh.CentreExtents[j].Z, true);
                    file.WriteFloat(mesh.CentreExtents[j].W, true);
                }

                ctx.AddReference(mesh.CentreExtents);

                file.WriteFloat(mesh.DensityDiscDiameter, true);

                ctx.AddReference(mesh);
            }
        }
    }
}
