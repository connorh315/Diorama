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

        public Vertex[] Vertices; // Defunct

        public byte[] VerticesDump;

        public int Stride;

        public VertexList(uint vertexCount, uint vertexDefinitionCount)
        {
            Vertices = new Vertex[vertexCount];

            Definitions = new VertexDefinition[vertexDefinitionCount];
        }

        public static VertexList Parse(RawFile file)
        {
            uint vertexCount = file.ReadUInt(true);

            Debug.Assert(file.ReadString(4) == "DXTV");
            Debug.Assert(file.ReadUInt(true) == 0xa9); // vtxd version

            uint vertexDefinitionCount = file.ReadUInt(true);
            
            VertexList list = new VertexList(vertexCount, vertexDefinitionCount);

            int stride = 0;
            for (int vDef = 0; vDef < vertexDefinitionCount; vDef++)
            {
                VertexDefinition def = VertexDefinition.Parse(file);

                list.Definitions[vDef] = def;
                stride += SizeOf(def.Type);
            }

            list.Stride = stride;

            byte[] instancingDividers = new byte[6];
            for (int i = 0; i < 6; i++)
                instancingDividers[i] = file.ReadByte();

            list.VerticesDump = file.ReadArray((int)(stride * vertexCount));

            return list;
        }

        private static int SizeOf(VertexDefinitionStorageEnum storage)
        {
            return storage switch
            {
                VertexDefinitionStorageEnum.vec2float => 8,
                VertexDefinitionStorageEnum.vec3float => 12,
                VertexDefinitionStorageEnum.vec4float => 16,
                VertexDefinitionStorageEnum.vec2half => 4,
                VertexDefinitionStorageEnum.vec4half => 8,
                VertexDefinitionStorageEnum.vec4char => 4,
                VertexDefinitionStorageEnum.vec4mini => 4,
                VertexDefinitionStorageEnum.color4char => 4,
                _ => 0 // TODO: Handle materials triggering this
            };
        }

        private Vertex ReadVertex(RawFile file)
        {
            Vertex vertex = new Vertex();
            foreach (VertexDefinition def in Definitions)
            {
                switch (def.Variable)
                {
                    case VertexDefinitionVariableEnum.position:
                        vertex.Position = ReadVector(file, def.Type).ToVector3();
                        break;
                    case VertexDefinitionVariableEnum.normal:
                        vertex.Normal = ReadVector(file, def.Type).ToVector3();
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
                    default:
                        ReadVector(file, def.Type); // discard, no implementation
                        break;
                }
            }
            return vertex;
        }

        private static Vector4 ReadVector(RawFile file, VertexDefinitionStorageEnum storageType)
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
        private static float NormalizeByte(byte value)
        {
            return value / 127.5f - 1.0f;
        }

    }
}
