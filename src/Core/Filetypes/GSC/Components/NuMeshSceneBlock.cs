using Diorama.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuMeshSceneBlock : ISchemaSerializable
    {
        public uint Version { get; set; }

        public NuRenderMesh[] Meshes { get; set; }

        protected VertexList GetVertexList(RawFile file, GSerializationContext ctx, ref uint flags, ref int offset)
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
                flags = file.ReadUInt(true); // 0x502??
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

        public void Read(RawFile file, GSerializationContext ctx)
        {
            Version = file.ReadUInt(true);
            Debug.Assert(Version == 0xaf || Version == 0xc8);

            Meshes = new NuRenderMesh[file.ReadInt(true)];
            for (int i = 0; i < Meshes.Length; i++)
            {
                Debug.Assert(file.ReadUInt(true) == 0x1);

                NuRenderMesh mesh = new NuRenderMesh();

                if (Version == 0xc8)
                {
                    Debug.Assert(file.ReadString(4) == "SMNR");
                    Debug.Assert(file.ReadInt(true) == 0xc8);
                }

                uint vertexBufferCount = file.ReadUInt(true);
                mesh.VertexBuffers = new VertexList[vertexBufferCount];
                mesh.VertexBufferFlags = new uint[vertexBufferCount];
                mesh.VertexBufferOffsets = new int[vertexBufferCount];

                for (int vList = 0; vList < vertexBufferCount; vList++)
                {
                    mesh.VertexBuffers[vList] = GetVertexList(file, ctx, ref mesh.VertexBufferFlags[vList], ref mesh.VertexBufferOffsets[vList]); // Not really sure what the "offset" field is / what it's for
                }

                uint fastBlendVbsCount = file.ReadUInt(true);
                mesh.FastBlendVBs = new VertexList[fastBlendVbsCount];
                mesh.FastBlendFlags = new uint[fastBlendVbsCount];
                mesh.FastBlendOffsets = new int[fastBlendVbsCount];
                for (int fastVList = 0; fastVList < fastBlendVbsCount; fastVList++)
                {
                    mesh.FastBlendVBs[fastVList] = GetVertexList(file, ctx, ref mesh.FastBlendFlags[fastVList], ref mesh.FastBlendOffsets[fastVList]);
                }
                if (fastBlendVbsCount != 0)
                {
                    ctx.AddReference(mesh.FastBlendVBs);
                }

                mesh.Indices = GetIndexList(file, ctx);
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
                    NuBlendShape.Parse(file, ctx, Version);
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

                Meshes[i] = mesh;
            }
        }

        public static NuMeshSceneBlock Parse(RawFile file, GSerializationContext ctx)
        {
            Debug.Assert(file.ReadString(4) == "HSEM");

            NuMeshSceneBlock meshBlock = new NuMeshSceneBlock();

            meshBlock.Read(file, ctx);

            return meshBlock;
        }

        public void Write(RawFile file, GSerializationContext ctx)
        {
            file.WriteUInt(Version, true);

            file.WriteInt(Meshes.Length, true);
            for (int i = 0; i < Meshes.Length; i++)
            {
                file.WriteInt(1, true);

                var mesh = Meshes[i];

                if (Version == 0xc8)
                {
                    file.WriteString("SMNR");
                    file.WriteInt(0xc8, true);
                }

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
                        uint flags = mesh.VertexBufferFlags[vList];
                        if (flags == 0) // safeguard
                        {
                            flags = 0x502;
                        }
                        file.WriteUInt(flags, true);
                        buffer.Write(file);
                        file.WriteInt(mesh.VertexBufferOffsets[vList], true);
                    }
                }

                if (mesh.FastBlendVBs != null)
                {
                    file.WriteInt(mesh.FastBlendVBs.Length, true);
                    for (int fastVList = 0; fastVList < mesh.FastBlendVBs.Length; fastVList++)
                    {
                        var buffer = mesh.FastBlendVBs[fastVList];
                        if (ctx.GetOrAddReference(buffer, out int reference))
                        {
                            file.WriteUInt((uint)(reference | 0xc0000000), true);
                            file.WriteUInt(1, true);
                            file.WriteInt(mesh.FastBlendOffsets[fastVList], true);
                        }
                        else
                        {
                            file.WriteUInt((uint)1, true);
                            file.WriteUInt(mesh.FastBlendFlags[fastVList], true);
                            buffer.Write(file);
                            file.WriteInt(mesh.FastBlendOffsets[fastVList], true);
                        }
                    }

                    if (mesh.FastBlendVBs.Length != 0) // not sure on this one really...
                    {
                        ctx.AddReference(mesh.FastBlendVBs);
                    }
                }
                else
                {
                    file.WriteInt(0);
                }

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

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.Expect("HSEM");

            if (schema.Writing)
            {
                Write(schema.File, (GSerializationContext)schema.Context);
            }
            else
            {
                Read(schema.File, (GSerializationContext)schema.Context);
            }
        }
    }
}
