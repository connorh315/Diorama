using Diorama.Core;
using Diorama.Core.Filetypes.GSC.Components;
using Diorama.Rendering;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Text.Json;

namespace Diorama.Editor.glTF
{
    public class glTFConverter
    {
        public static void WriteObjectsToGltf(IEnumerable<EditorGeometryObject> objects, string path)
        {
            var geometries = objects.ToList();

            if (geometries.Count == 0)
                return;

            glTFHeader header = new glTFHeader();
            header.AddScene();

            string binPath = Path.ChangeExtension(path, "bin");

            using (RawFile binary = RawFile.Create(binPath))
            {
                var character = geometries[0].HighestDetail;

                int skinIndex = -1;
                if (character != null)
                {
                    skinIndex = WriteSkin(header, binary, character);
                }

                foreach (var geo in geometries)
                {
                    WriteMesh(header, binary, geo, skinIndex);
                }

                header.AddBuffer(Path.GetFileName(binPath), (int)binary.Position);
            }

            header.WriteToFile(path);
        }

        private static int WriteMesh(glTFHeader header, RawFile binary, EditorGeometryObject geo, int skinIndex)
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
                        if (attr.Type == VertexDefinitionStorageEnum.vec4half)
                        {
                            attributeOffsets.Add($"{entry}1", -1);
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

        private static int WriteSkin(glTFHeader header, RawFile binary, NuCharacterData character)
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

        private static GltfComponentType GetComponentType(string semantic)
        {
            return semantic switch
            {
                "JOINTS_0" => GltfComponentType.UnsignedShort,
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
                VertexDefinitionVariableEnum.uvSet2 => "TEXCOORD_2",
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
                    file.WriteVector2(v.UVSet02, false);
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
                    file.WriteUShort(remapBones[v.BlendIndices.X], false);
                    file.WriteUShort(remapBones[v.BlendIndices.Y], false);
                    file.WriteUShort(remapBones[v.BlendIndices.Z], false);
                    file.WriteUShort(remapBones[v.BlendIndices.W], false);
                }
            }
        }

        public static void Icosahedron()
        {
            float t = (1.0f + MathF.Sqrt(5.0f)) / 2.0f;

            Vector3[] vertices =
            {
                new(-1,  t,  0),
                new( 1,  t,  0),
                new(-1, -t,  0),
                new( 1, -t,  0),

                new( 0, -1,  t),
                new( 0,  1,  t),
                new( 0, -1, -t),
                new( 0,  1, -t),

                new( t,  0, -1),
                new( t,  0,  1),
                new(-t,  0, -1),
                new(-t,  0,  1)
            };

            ushort[] indices =
            {
                 0, 11,  5,
                 0,  5,  1,
                 0,  1,  7,
                 0,  7, 10,
                 0, 10, 11,

                 1,  5,  9,
                 5, 11,  4,
                11, 10,  2,
                10,  7,  6,
                 7,  1,  8,

                 3,  9,  4,
                 3,  4,  2,
                 3,  2,  6,
                 3,  6,  8,
                 3,  8,  9,

                 4,  9,  5,
                 2,  4, 11,
                 6,  2, 10,
                 8,  6,  7,
                 9,  8,  1
            };

            glTFHeader header = new glTFHeader()
            {
                Asset = new Asset
                {
                    Generator = $"{AppSettings.AppString} glTF exporter",
                },
                Scene = 0,
                Scenes = [new Scene {  }],
                Nodes = [new Node { Name = "IcosahedronABC", Mesh = 0 }],
                Meshes = new()
            };

            GltfMesh mesh = new GltfMesh()
            {
                Name = "Icosahedron123",
                Primitives = new()
            };

            mesh.Primitives.Add(new GltfPrimitive
            {
                Attributes = new Dictionary<string, int> { ["POSITION"] = 0 },
                Indices = 1,
                Mode = 4
            });

            header.Meshes.Add(mesh);

            header.Accessors = 
            [
                new GltfAccessor 
                {
                    BufferView = 0,
                    ComponentType = (int)GltfComponentType.Float,
                    Count = 12,
                    Type = "VEC3",

                    Min = [-1.618034f, -1.618034f, -1.618034f],
                    Max = [ 1.618034f,  1.618034f,  1.618034f]
                },

                new GltfAccessor 
                {
                    BufferView = 1,
                    ComponentType = (int)GltfComponentType.UnsignedShort,
                    Count = 60,
                    Type = "SCALAR"
                }
            ];

            header.BufferViews =
            [
                new GltfBufferView 
                {
                    Buffer = 0,
                    ByteOffset = 0,
                    ByteLength = 12 * 3 * sizeof(float),
                    Target = 34692 // array_buffer
                },

                new GltfBufferView 
                {
                    Buffer = 0,
                    ByteOffset = 144,
                    ByteLength = 60 * sizeof(ushort),
                    Target = 34963 // element_array_buffer
                }
            ];

            header.Buffers =
            [
                new GltfBuffer 
                {
                    Uri = "icosahedron.bin",
                    ByteLength = 264
                }
            ];

            using (var stream = File.Create("A:\\icosahedron.bin"))
            using (var writer = new BinaryWriter(stream))
            {
                foreach (Vector3 vertex in vertices)
                {
                    writer.Write(vertex.X);
                    writer.Write(vertex.Y);
                    writer.Write(vertex.Z);
                }

                foreach (ushort index in indices)
                {
                    writer.Write(index);
                }
            }
        }
    }
}
