using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class VertexList
    {
        public VertexDefinition[] Definitions;

        public uint VerticesCount;

        public byte[] VerticesDump;

        public int Stride;

        public byte[] InstancingDividers = new byte[6];

        public VertexList(uint vertexCount, uint vertexDefinitionCount)
        {
            VerticesCount = vertexCount;

            Definitions = new VertexDefinition[vertexDefinitionCount];
        }

        public static VertexList ParseHeader(RawFile file, uint vertexCount = 0)
        {
            Debug.Assert(file.ReadString(4) == "DXTV");
            Debug.Assert(file.ReadUInt(true) == 0xa9); // vtxd version

            uint vertexDefinitionCount = file.ReadUInt(true);

            VertexList list = new VertexList(vertexCount, vertexDefinitionCount);

            int stride = 0;
            for (int vDef = 0; vDef < vertexDefinitionCount; vDef++)
            { // if def.type is like 0x26 etc. I think it might mean to use the second vertex buffer + then the specific type??
                VertexDefinition def = VertexDefinition.Parse(file);

                list.Definitions[vDef] = def;
                stride += SizeOf(def.Type);
            }

            list.Stride = stride;

            for (int i = 0; i < 6; i++)
                list.InstancingDividers[i] = file.ReadByte();

            return list;
        }

        public static VertexList Parse(RawFile file)
        {
            uint vertexCount = file.ReadUInt(true);

            VertexList list = ParseHeader(file, vertexCount);

            list.VerticesDump = file.ReadArray((int)(list.Stride * vertexCount));

            return list;
        }

        public void WriteHeader(RawFile file)
        {
            file.WriteString("DXTV");
            file.WriteInt(0xa9, true);

            file.WriteInt(Definitions.Length, true);

            foreach (var def in Definitions)
            {
                def.Write(file);
            }

            for (int i = 0; i < 6; i++)
                file.WriteByte(InstancingDividers[i]); // instancing dividers, rarely used
        }

        public void Write(RawFile file)
        {
            file.WriteUInt(VerticesCount, true);

            WriteHeader(file);

            file.WriteArray(VerticesDump);
        }

        private static int SizeOf(VertexDefinitionStorageEnum storage)
        {
            return ((VertexDefinitionStorageEnum)((int)storage & 0xf)) switch
            {
                VertexDefinitionStorageEnum.vec2float => 8,
                VertexDefinitionStorageEnum.vec3float => 12,
                VertexDefinitionStorageEnum.vec4float => 16,
                VertexDefinitionStorageEnum.vec2half => 4,
                VertexDefinitionStorageEnum.vec4half => 8,
                VertexDefinitionStorageEnum.vec4char => 4,
                VertexDefinitionStorageEnum.vec4mini => 4,
                VertexDefinitionStorageEnum.color4char => 4,
                _ => throw new Exception("Unknown storage type") // TODO: Handle materials triggering this
            };
        }

        public void WriteVertex(RawFile file, Vertex vertex)
        {
            foreach (VertexDefinition def in Definitions)
            {
                switch (def.Variable)
                {
                    case VertexDefinitionVariableEnum.position:
                        WriteVector(file, new Vector4(vertex.Position, 1f), def.Type);
                        break;
                    case VertexDefinitionVariableEnum.normal:
                        WriteVector(file, new Vector4(vertex.Normal, 1f), def.Type, true);
                        break;
                    case VertexDefinitionVariableEnum.tangent:
                        WriteVector(file, new Vector4(vertex.Tangent, 1), def.Type, true);
                        break;
                    case VertexDefinitionVariableEnum.colorSet0:
                        WriteVector(file, new Vector4(vertex.ColorSet0.Z, vertex.ColorSet0.Y, vertex.ColorSet0.X, vertex.ColorSet0.W), def.Type);
                        break;
                    case VertexDefinitionVariableEnum.colorSet1:
                        WriteVector(file, vertex.ColorSet1, def.Type);
                        break;
                    case VertexDefinitionVariableEnum.uvSet01:
                        WriteVector(file, vertex.UVSet01, def.Type);
                        break;
                    case VertexDefinitionVariableEnum.uvSet2:
                        WriteVector(file, vertex.UVSet23, def.Type);
                        break;
                    case VertexDefinitionVariableEnum.blendIndices0:
                        WriteVectorI(file, vertex.BlendIndices);
                        break;
                    case VertexDefinitionVariableEnum.blendWeight0:
                        WriteVector(file, vertex.BlendWeights, def.Type, false);
                        break;
                    default:
                        WriteVector(file, Vector4.Zero, def.Type); // discard, no implementation
                        break;
                }
            }
        }

        public void WriteVectorI(RawFile file, VectorI4 vec)
        {
            file.WriteByte((byte)vec.X);
            file.WriteByte((byte)vec.Y);
            file.WriteByte((byte)vec.Z);
            file.WriteByte((byte)vec.W);
        }

        public void WriteVector(RawFile file, Vector4 vec, VertexDefinitionStorageEnum storage, bool signedValue = false)
        {
            switch (storage)
            {
                case VertexDefinitionStorageEnum.vec2float:
                    file.WriteFloat(vec.X, false);
                    file.WriteFloat(vec.Y, false);
                    break;
                case VertexDefinitionStorageEnum.vec3float:
                    file.WriteFloat(vec.X, false);
                    file.WriteFloat(vec.Y, false);
                    file.WriteFloat(vec.Z, false);
                    break;
                case VertexDefinitionStorageEnum.vec4float:
                    file.WriteFloat(vec.X, false);
                    file.WriteFloat(vec.Y, false);
                    file.WriteFloat(vec.Z, false);
                    file.WriteFloat(vec.W, false);
                    break;
                case VertexDefinitionStorageEnum.vec2half:
                    file.WriteHalf((Half)vec.X, false);
                    file.WriteHalf((Half)vec.Y, false);
                    break;
                case VertexDefinitionStorageEnum.vec4half:
                    file.WriteHalf((Half)vec.X, false);
                    file.WriteHalf((Half)vec.Y, false);
                    file.WriteHalf((Half)vec.Z, false);
                    file.WriteHalf((Half)vec.W, false);
                    break;
                case VertexDefinitionStorageEnum.vec4mini:
                case VertexDefinitionStorageEnum.vec4char:
                    if (signedValue)
                    {
                        file.WriteByte(NormalizeSignedFloat(vec.X));
                        file.WriteByte(NormalizeSignedFloat(vec.Y));
                        file.WriteByte(NormalizeSignedFloat(vec.Z));
                        file.WriteByte(NormalizeSignedFloat(vec.W));
                    }
                    else
                    {
                        file.WriteByte(NormalizeFloat(vec.X));
                        file.WriteByte(NormalizeFloat(vec.Y));
                        file.WriteByte(NormalizeFloat(vec.Z));
                        file.WriteByte(NormalizeFloat(vec.W));
                    }
                    break;
                case VertexDefinitionStorageEnum.color4char:
                    file.WriteByte((byte)(vec.X * 255));
                    file.WriteByte((byte)(vec.Y * 255));
                    file.WriteByte((byte)(vec.Z * 255));
                    file.WriteByte((byte)(vec.W * 255));
                    break;
            }
        }

        public byte NormalizeFloat(float val) => (byte)MathF.Round(val * 255);

        public byte NormalizeSignedFloat(float val) => (byte)MathF.Round((val + 1) * 127.5f);

        public static VertexList FromVertices(List<Vertex> vertices, VertexDefinition[] definitions)
        {
            VertexList list = new VertexList((uint)vertices.Count, (uint)definitions.Length);

            int stride = 0;
            for (int i = 0; i < definitions.Length; i++)
            {
                var def = definitions[i];
                list.Definitions[i] = def;

                stride += SizeOf(def.Type);
            }
            list.Stride = stride;

            byte[] buffer;
            using (RawFile file = new RawFile(new MemoryStream()))
            {
                foreach (var vertex in vertices)
                {
                    list.WriteVertex(file, vertex);
                }

                buffer = ((MemoryStream)file.fileStream).ToArray();
            }

            list.VerticesDump = buffer;

            return list;
        }

        public static Vertex[] CreateVerticesArray(int count)
        {
            Vertex[] list = new Vertex[count];
            for (int i = 0; i < count; i++)
            {
                list[i] = new Vertex();
            }
            return list;
        }

        public void FillVertices(ref Vertex[] vertices, int firstVertexIndex)
        {
            int bufferStartOffset = firstVertexIndex * Stride;

            using (MemoryStream stream = new MemoryStream(VerticesDump))
            using (RawFile file = new RawFile(stream))
            {
                file.Seek(bufferStartOffset, SeekOrigin.Begin);
                for (int i = 0; i < vertices.Length; i++)
                {
                    ReadVertex(file, ref vertices[i]);
                }
            }
        }

        private void ReadVertex(RawFile file, ref Vertex vertex)
        {
            foreach (VertexDefinition def in Definitions)
            {
                switch (def.Variable)
                {
                    case VertexDefinitionVariableEnum.position:
                        vertex.Position = ReadVector(file, def.Type).ToVector3();
                        break;
                    case VertexDefinitionVariableEnum.normal:
                        vertex.Normal = ReadVector(file, def.Type, true).ToVector3();
                        break;
                    case VertexDefinitionVariableEnum.tangent:
                        vertex.Tangent = ReadVector(file, def.Type, true).ToVector3();
                        break;
                    case VertexDefinitionVariableEnum.colorSet0:
                        vertex.ColorSet0 = ReadVector(file, def.Type);
                        break;
                    case VertexDefinitionVariableEnum.colorSet1:
                        vertex.ColorSet1 = ReadVector(file, def.Type);
                        break;
                    case VertexDefinitionVariableEnum.uvSet01:
                        vertex.UVSet01 = ReadVector(file, def.Type);
                        break;
                    case VertexDefinitionVariableEnum.uvSet2:
                        vertex.UVSet23 = ReadVector(file, def.Type);
                        break;
                    case VertexDefinitionVariableEnum.blendWeight0:
                        vertex.BlendWeights = ReadVector(file, def.Type, false);
                        break;
                    case VertexDefinitionVariableEnum.blendIndices0:
                        vertex.BlendIndices = new VectorI4(file.ReadByte(), file.ReadByte(), file.ReadByte(), file.ReadByte()); // assumption here that it's always vec4char
                        break;
                    default:
                        ReadVector(file, def.Type); // discard, no implementation
                        break;
                }
            }
        }

        private static Vector4 ReadVector(RawFile file, VertexDefinitionStorageEnum storageType, bool signedValue = false)
        {
            switch (storageType)
            {
                case VertexDefinitionStorageEnum.vec2float:
                    return new Vector4(
                        file.ReadFloat(false),
                        file.ReadFloat(false),
                        0f,
                        1f);

                case VertexDefinitionStorageEnum.vec3float:
                    return new Vector4(
                        file.ReadFloat(false),
                        file.ReadFloat(false),
                        file.ReadFloat(false),
                        1f);

                case VertexDefinitionStorageEnum.vec4float:
                    return new Vector4(
                        file.ReadFloat(false),
                        file.ReadFloat(false),
                        file.ReadFloat(false),
                        file.ReadFloat(false));

                case VertexDefinitionStorageEnum.vec2half:
                    return new Vector4(
                        (float)file.ReadHalf(false),
                        (float)file.ReadHalf(false),
                        0f,
                        1f);

                case VertexDefinitionStorageEnum.vec4half:
                    return new Vector4(
                        (float)file.ReadHalf(false),
                        (float)file.ReadHalf(false),
                        (float)file.ReadHalf(false),
                        (float)file.ReadHalf(false));

                case VertexDefinitionStorageEnum.vec4mini:
                    if (signedValue)
                        return new Vector4(
                            NormalizeSignedByte(file.ReadByte()),
                            NormalizeSignedByte(file.ReadByte()),
                            NormalizeSignedByte(file.ReadByte()),
                            NormalizeSignedByte(file.ReadByte()));
                    else
                        return new Vector4(
                            NormalizeByte(file.ReadByte()),
                            NormalizeByte(file.ReadByte()),
                            NormalizeByte(file.ReadByte()),
                            NormalizeByte(file.ReadByte()));

                case VertexDefinitionStorageEnum.vec4char:
                    return new Vector4(file.ReadByte(), file.ReadByte(), file.ReadByte(), file.ReadByte());

                case VertexDefinitionStorageEnum.color4char:
                    return new Vector4((float)file.ReadByte()/255, (float)file.ReadByte() / 255, (float)file.ReadByte() / 255, (float)file.ReadByte() / 255);

                default:
                    throw new NotSupportedException($"Unsupported storage type {storageType}");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float NormalizeSignedByte(byte value)
        {
            return value / 127.5f - 1.0f;
        }

        private static float NormalizeByte(byte value)
        {
            return value / 255f;
        }
    }
}
