using Diorama.Core.Filetypes.GSC.Components;
using Diorama.Rendering;
using Diorama.Core;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Reflection.PortableExecutable;

namespace Diorama.Editor.glTF
{
    public class glTFBinary
    {
        public static int WriteMesh(glTFHeader header, RawFile binary, EditorGeometryObject geo, int skinIndex)
        {
            var mesh = geo.Mesh;
            var nuMesh = mesh.OriginalMesh;

            Vertex[] vertices = VertexList.CreateVerticesArray(mesh.VerticesCount);

            for (int i = 0; i < nuMesh.VertexBuffers.Length; i++)
            {
                nuMesh.VertexBuffers[i].FillVertices(ref vertices, mesh.VerticesBase);
            }

            Dictionary<string, int> attributeOffsets = GetAttributeOffsets(mesh);

            Vector3 min;
            Vector3 max;

            WriteVertices(binary, vertices, nuMesh.SkinMtxMap, attributeOffsets, out min, out max);

            var primitive = new GltfPrimitive
            {
                Attributes = new(),
                Mode = 4
            };

            foreach (var attr in attributeOffsets)
            {
                if (attr.Value == -1)
                    continue;

                int stride = GetAttributeStride(attr.Key);

                int bufferViewIndex = header.AddBufferView(new GltfBufferView
                {
                    Buffer = 0,
                    ByteOffset = attr.Value,
                    ByteLength = vertices.Length * stride,
                    Target = 34962
                });

                var accessor = new GltfAccessor
                {
                    BufferView = bufferViewIndex,
                    ComponentType = (int)GetComponentType(attr.Key),
                    Count = vertices.Length,
                    Type = GetAccessorType(attr.Key)
                };

                if (attr.Key == "POSITION")
                {
                    accessor.Min = [min.X, min.Y, min.Z];
                    accessor.Max = [max.X, max.Y, max.Z];
                }

                int accessorIndex = header.AddAccessor(accessor);

                primitive.Attributes[attr.Key] = accessorIndex;
            }

            int indicesOffset = (int)binary.Position;

            for (int i = (int)nuMesh.IndicesBase;
                 i < nuMesh.IndicesBase + nuMesh.IndicesCount;
                 i++)
            {
                binary.WriteUShort(nuMesh.Indices[i], false);
            }

            int indexBufferView = header.AddBufferView(new GltfBufferView
            {
                Buffer = 0,
                ByteOffset = indicesOffset,
                ByteLength = (int)nuMesh.IndicesCount * sizeof(ushort),
                Target = 34963
            });

            int indexAccessor = header.AddAccessor(new GltfAccessor
            {
                BufferView = indexBufferView,
                ComponentType = (int)GltfComponentType.UnsignedShort,
                Count = (int)nuMesh.IndicesCount,
                Type = "SCALAR"
            });

            primitive.Indices = indexAccessor;

            string meshName = geo.Parent?.Parent?.Name ?? "diorama_exported_object";

            int meshIndex = header.AddMesh(meshName, new() { primitive });

            var node = header.AddMeshNodeToScene(0, meshName);
            node.Mesh = meshIndex;

            if (nuMesh.SkinMtxMap != null && skinIndex != -1)
            {
                node.Skin = skinIndex;
            }

            node.Matrix = geo.Transform.ToList();

            return meshIndex;
        }

        public static int WriteSkin(glTFHeader header, RawFile binary, NuCharacterData character)
        {
            int[] jointNodeIndices = new int[character.JointData.Count];

            for (int i = 0; i < character.JointData.Count; i++)
            {
                NuJointData joint = character.JointData[i];

                int nodeIndex = header.Nodes.Count;
                jointNodeIndices[i] = nodeIndex;

                header.Nodes.Add(new Node
                {
                    Name = joint.Name,
                    Children = new List<int>(),
                    Matrix = character.T[i].mtx.ToList()
                });
            }

            int rootJointNode = -1;

            for (int i = 0; i < character.JointData.Count; i++)
            {
                NuJointData joint = character.JointData[i];

                if (joint.ParentIndex == 0xff)
                {
                    rootJointNode = jointNodeIndices[i];
                    continue;
                }

                int parentNode = jointNodeIndices[joint.ParentIndex];
                int childNode = jointNodeIndices[i];

                header.Nodes[parentNode].Children.Add(childNode);
            }

            int inverseMatricesOffset = (int)binary.Position;

            foreach (NuMtx mtx in character.Inv_Wt)
                mtx.Serialize(binary, false);

            int inverseBufferView = header.AddBufferView(new GltfBufferView
            {
                Buffer = 0,
                ByteOffset = inverseMatricesOffset,
                ByteLength = character.Inv_Wt.Count * 16 * sizeof(float)
            });

            int inverseBindAccessor = header.AddAccessor(new GltfAccessor
            {
                BufferView = inverseBufferView,
                ComponentType = (int)GltfComponentType.Float,
                Count = character.Inv_Wt.Count,
                Type = "MAT4"
            });

            int skinIndex = header.AddSkin(new GltfSkin
            {
                Name = "Armature",
                Skeleton = rootJointNode,
                Joints = jointNodeIndices.ToList(),
                InverseBindMatrices = inverseBindAccessor
            });

            header.Scenes[0].Nodes.Add(rootJointNode);

            return skinIndex;
        }

        public static List<T> GetFromAccessor<T>(RawFile binary, glTFHeader header, int accessorIndex)
        {
            GltfAccessor accessor = header.Accessors[accessorIndex];

            GltfBufferView view = header.BufferViews[accessor.BufferView];

            binary.Seek(view.ByteOffset, SeekOrigin.Begin);

            List<T> list = new List<T>();

            for (int i = 0; i < accessor.Count; i++)
            {
                object value;

                if (typeof(T) == typeof(Vector2))
                    value = binary.ReadVector2();
                else if (typeof(T) == typeof(Vector3))
                    value = binary.ReadVector3();
                else if (typeof(T) == typeof(Vector4))
                {
                    if (accessor.Type == "VEC4")
                    {
                        switch ((GltfComponentType)accessor.ComponentType)
                        {
                            case GltfComponentType.UnsignedByte:
                                value = new Vector4((float)binary.ReadByte() / 255, (float)binary.ReadByte() / 255, (float)binary.ReadByte() / 255, (float)binary.ReadByte() / 255);
                                break;
                            case GltfComponentType.UnsignedShort:
                                value = new Vector4((float)binary.ReadUShort() / 65535, (float)binary.ReadUShort() / 65535, (float)binary.ReadUShort() / 65535, (float)binary.ReadUShort() / 65535);
                                break;
                            case GltfComponentType.Float:
                                value = binary.ReadVector4();
                                break;
                            default:
                                throw new Exception();
                        }
                    }
                    else if (accessor.Type == "VEC3")
                    {
                        switch ((GltfComponentType)accessor.ComponentType)
                        {
                            case GltfComponentType.UnsignedByte:
                                value = new Vector4((float)binary.ReadByte() / 255, (float)binary.ReadByte() / 255, (float)binary.ReadByte() / 255, 1);
                                break;
                            case GltfComponentType.UnsignedShort:
                                value = new Vector4((float)binary.ReadUShort() / 65535, (float)binary.ReadUShort() / 65535, (float)binary.ReadUShort() / 65535, 1);
                                break;
                            case GltfComponentType.Float:
                                Vector3 val = binary.ReadVector3();
                                value = new Vector4(val, 1);
                                break;
                            default:
                                throw new Exception();
                        }
                    }
                    else
                        throw new InvalidDataException($"Defined a {accessor.Type} for a Vector4 structure.");
                }
                else if (typeof(T) == typeof(VectorI4))
                    value = new VectorI4(binary.ReadByte(), binary.ReadByte(), binary.ReadByte(), binary.ReadByte());
                else if (typeof(T) == typeof(float))
                    value = binary.ReadFloat();
                else if (typeof(T) == typeof(uint))
                    value = binary.ReadUInt();
                else if (typeof(T) == typeof(ushort))
                    value = binary.ReadUShort();
                else
                    throw new NotSupportedException($"glTF Accessor type {typeof(T).Name} is not supported.");

                list.Add((T)value);
            }

            return list;
        }

        public static List<ushort> GetIndices(
            RawFile binary,
            glTFHeader header,
            int accessorIndex)
        {
            GltfAccessor accessor = header.Accessors[accessorIndex];

            return (GltfComponentType)accessor.ComponentType switch
            {
                GltfComponentType.UnsignedByte => GetFromAccessor<byte>(binary, header, accessorIndex)
                            .Select(x => (ushort)x)
                            .ToList(),

                GltfComponentType.UnsignedShort => GetFromAccessor<ushort>(binary, header, accessorIndex),

                GltfComponentType.UnsignedInt => GetFromAccessor<uint>(binary, header, accessorIndex)
                            .Select(x => (ushort)x)
                            .ToList(),

                _ => throw new NotSupportedException(
                    $"Unsupported index component type: {accessor.ComponentType}")
            };
        }

        private static Dictionary<string, int> GetAttributeOffsets(RenderMesh mesh)
        {
            Dictionary<string, int> attributeOffsets = new();

            foreach (var buf in mesh.VertexBuffers)
            {
                foreach (var attr in buf.Attributes)
                {
                    string entry = GetGltfSemantic(attr.Variable);
                    if (entry == "TEXCOORD_")
                    {
                        attributeOffsets.Add($"{entry}0", -1);
                        if (attr.Type == VertexDefinitionStorageEnum.vec4half || attr.Type == VertexDefinitionStorageEnum.vec4float)
                        {
                            attributeOffsets.Add($"{entry}1", -1);
                        }
                    }
                    else if (entry == "TEXCOORD__")
                    {
                        attributeOffsets.Add($"TEXCOORD_2", -1);
                        if (attr.Type == VertexDefinitionStorageEnum.vec4half || attr.Type == VertexDefinitionStorageEnum.vec4float)
                        {
                            attributeOffsets.Add($"TEXCOORD_3", -1);
                        }
                    }
                    else
                    {
                        attributeOffsets.Add(entry, -1);
                    }
                }
            }

            return attributeOffsets;
        }

        private static GltfComponentType GetComponentType(string semantic)
        {
            return semantic switch
            {
                "JOINTS_0" => GltfComponentType.UnsignedByte,
                _ => GltfComponentType.Float
            };
        }

        private static string GetAccessorType(string semantic)
        {
            return semantic switch
            {
                "POSITION" => "VEC3",
                "NORMAL" => "VEC3",
                "TANGENT" => "VEC3", // see caveat below
                "COLOR_0" => "VEC4",
                "COLOR_1" => "VEC4",
                "TEXCOORD_0" => "VEC2",
                "TEXCOORD_1" => "VEC2",
                "TEXCOORD_2" => "VEC2",
                "TEXCOORD_3" => "VEC2",
                "WEIGHTS_0" => "VEC4",
                "JOINTS_0" => "VEC4",
                _ => throw new NotSupportedException(semantic)
            };
        }

        private static string GetGltfSemantic(VertexDefinitionVariableEnum variable) =>
            variable switch
            {
                VertexDefinitionVariableEnum.position => "POSITION",
                VertexDefinitionVariableEnum.normal => "NORMAL",
                VertexDefinitionVariableEnum.colorSet0 => "COLOR_0",
                VertexDefinitionVariableEnum.tangent => "TANGENT",
                VertexDefinitionVariableEnum.colorSet1 => "COLOR_1",
                VertexDefinitionVariableEnum.uvSet01 => "TEXCOORD_",
                VertexDefinitionVariableEnum.diffuse => "_DIFFUSE",
                VertexDefinitionVariableEnum.uvSet2 => "TEXCOORD__",
                VertexDefinitionVariableEnum.albedo => "_ALBEDO",
                VertexDefinitionVariableEnum.blendIndices0 => "JOINTS_0",
                VertexDefinitionVariableEnum.blendWeight0 => "WEIGHTS_0",
                VertexDefinitionVariableEnum.tangent2 => "_TANGENT_2",
                VertexDefinitionVariableEnum.lightDirSet => "_LIGHTDIRSET",
                VertexDefinitionVariableEnum.lightColSet => "_LIGHTCOLSET",
            };

        private static int GetAttributeStride(string semantic)
        {
            return semantic switch
            {
                "POSITION" => 12,
                "NORMAL" => 12,
                "TANGENT" => 12,
                "COLOR_0" => 16,
                "COLOR_1" => 16,
                "TEXCOORD_0" => 8,
                "TEXCOORD_1" => 8,
                "TEXCOORD_2" => 8,
                "TEXCOORD_3" => 8,
                "WEIGHTS_0" => 16,
                "JOINTS_0" => 8,
                _ => throw new NotSupportedException(semantic)
            };
        }

        private static void WriteVertices(RawFile file, Vertex[] vertices, List<byte> remapBones, Dictionary<string, int> offsets, out Vector3 min, out Vector3 max)
        {
            min = Vector3.PositiveInfinity;
            max = Vector3.NegativeInfinity;

            if (offsets.ContainsKey("POSITION"))
            {
                offsets["POSITION"] = (int)file.Position;

                foreach (Vertex v in vertices)
                {
                    file.WriteVector3(v.Position, false);

                    min = Vector3.Min(min, v.Position);
                    max = Vector3.Max(max, v.Position);
                }
            }

            if (offsets.ContainsKey("NORMAL"))
            {
                offsets["NORMAL"] = (int)file.Position;

                foreach (Vertex v in vertices)
                    file.WriteVector3(v.Normal, false);
            }

            if (offsets.ContainsKey("COLOR_0"))
            {
                offsets["COLOR_0"] = (int)file.Position;

                foreach (Vertex v in vertices)
                    file.WriteVector4(v.ColorSet0, false);
            }

            if (offsets.ContainsKey("COLOR_1"))
            {
                offsets["COLOR_1"] = (int)file.Position;

                foreach (Vertex v in vertices)
                    file.WriteVector4(v.ColorSet1, false);
            }

            if (offsets.ContainsKey("TANGENT"))
            {
                offsets["TANGENT"] = (int)file.Position;

                foreach (Vertex v in vertices)
                    file.WriteVector3(v.Tangent, false);
            }

            if (offsets.ContainsKey("TEXCOORD_0"))
            {
                offsets["TEXCOORD_0"] = (int)file.Position;

                foreach (Vertex v in vertices)
                    file.WriteVector2(
                        new Vector2(v.UVSet01.X, v.UVSet01.Y),
                        false);
            }

            if (offsets.ContainsKey("TEXCOORD_1"))
            {
                offsets["TEXCOORD_1"] = (int)file.Position;

                foreach (Vertex v in vertices)
                    file.WriteVector2(
                        new Vector2(v.UVSet01.Z, v.UVSet01.W),
                        false);
            }

            if (offsets.ContainsKey("TEXCOORD_2"))
            {
                offsets["TEXCOORD_2"] = (int)file.Position;

                foreach (Vertex v in vertices)
                    file.WriteVector2(new Vector2(v.UVSet23.X, v.UVSet23.Y), false);
            }

            if (offsets.ContainsKey("TEXCOORD_3"))
            {
                offsets["TEXCOORD_3"] = (int)file.Position;

                foreach (Vertex v in vertices)
                    file.WriteVector2(new Vector2(v.UVSet23.Z, v.UVSet23.W), false);
            }

            if (offsets.ContainsKey("WEIGHTS_0"))
            {
                offsets["WEIGHTS_0"] = (int)file.Position;

                foreach (Vertex v in vertices)
                    file.WriteVector4(v.BlendWeights, false);
            }

            if (offsets.ContainsKey("JOINTS_0"))
            {
                offsets["JOINTS_0"] = (int)file.Position;

                foreach (Vertex v in vertices)
                {
                    file.WriteByte(remapBones[v.BlendIndices.X]);
                    file.WriteByte(remapBones[v.BlendIndices.Y]);
                    file.WriteByte(remapBones[v.BlendIndices.Z]);
                    file.WriteByte(remapBones[v.BlendIndices.W]);
                }
            }
        }
    }
}
