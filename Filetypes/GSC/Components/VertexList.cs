using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Filetypes.GSC.Components
{
    public class VertexList
    {
        public VertexDefinition[] Definitions;

        public Vertex[] Vertices;

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

            for (int vDef = 0; vDef < vertexDefinitionCount; vDef++)
            {
                VertexDefinition def = VertexDefinition.Parse(file);

                list.Definitions[vDef] = def;

                Console.WriteLine($"Vertex Definition {vDef}: Variable={def.Variable}, Type={def.Type}, Offset={def.Offset}");
            }

            Debug.Assert(file.ReadUInt(true) == 0);
            Debug.Assert(file.ReadUShort(true) == 0); // 6 bytes of padding

            for (int vertexIndex = 0; vertexIndex < vertexCount; vertexIndex++)
            {
                list.Vertices[vertexIndex] = list.ReadVertex(file);
            }

            return list;
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
                        vertex.UVSet0 = ReadVector(file, def.Type).ToVector2();
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
                        file.ReadFloat(true),
                        file.ReadFloat(true),
                        0f,
                        1f);

                case VertexDefinitionStorageEnum.vec3float:
                    return new Vector4(
                        file.ReadFloat(true),
                        file.ReadFloat(true),
                        file.ReadFloat(true),
                        1f);

                case VertexDefinitionStorageEnum.vec4float:
                    return new Vector4(
                        file.ReadFloat(true),
                        file.ReadFloat(true),
                        file.ReadFloat(true),
                        file.ReadFloat(true));

                case VertexDefinitionStorageEnum.vec2half:
                    return new Vector4(
                        (float)file.ReadHalf(true),
                        (float)file.ReadHalf(true),
                        0f,
                        1f);

                case VertexDefinitionStorageEnum.vec4half:
                    return new Vector4(
                        (float)file.ReadHalf(true),
                        (float)file.ReadHalf(true),
                        (float)file.ReadHalf(true),
                        (float)file.ReadHalf(true));

                case VertexDefinitionStorageEnum.vec4mini:
                    return new Vector4(
                        NormalizeByte(file.ReadByte()),
                        NormalizeByte(file.ReadByte()),
                        NormalizeByte(file.ReadByte()),
                        NormalizeByte(file.ReadByte()));

                case VertexDefinitionStorageEnum.vec4char:
                    file.Seek(4, SeekOrigin.Current);
                    return Vector4.Zero;

                case VertexDefinitionStorageEnum.color4char:
                    return new Vector4((float)file.ReadByte()/255, (float)file.ReadByte() / 255, (float)file.ReadByte() / 255, (float)file.ReadByte() / 255);

                default:
                    throw new NotSupportedException($"Unsupported storage type {storageType}");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float NormalizeByte(byte value)
        {
            return (value / 127.5f) - 1.0f;
        }

    }
}
