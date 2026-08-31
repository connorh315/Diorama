using Diorama.Core;
using Diorama.Core.Filetypes.GSC;
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
                    skinIndex = glTFBinary.WriteSkin(header, binary, character);
                }

                foreach (var geo in geometries)
                {
                    glTFBinary.WriteMesh(header, binary, geo, skinIndex);
                }

                header.AddBuffer(Path.GetFileName(binPath), (int)binary.Position);
            }

            header.WriteToFile(path);
        }

        public static RenderMesh GetObjectsFromGltf(string path, RenderMesh originalMesh, EditorScene scene)
        {
            glTFHeader header = glTFHeader.ReadFromFile(path);

            string binaryPath = Path.Join(Path.GetDirectoryName(path), header.Buffers[0].Uri);

            Node mainNode = null;

            var sceneNodes = header.Scenes[0].Nodes;

            for (int i = 0; i < sceneNodes.Count; i++)
            {
                Node baseNode = header.Nodes[header.Scenes[0].Nodes[i]];
                if (baseNode.Children != null)
                {
                    foreach (int childNodeIndex in baseNode.Children)
                    {
                        Node child = header.Nodes[childNodeIndex];

                        if (child.Mesh != null || child.Skin != null)
                        {
                            mainNode = child;
                            break;
                        }
                    }
                }

                if (mainNode == null && baseNode.Mesh != null)
                {
                    mainNode = baseNode;
                }
            }

            if (mainNode == null)
                throw new Exception($"Could not locate main node in scene!");

            if (!Path.Exists(binaryPath))
                throw new FileNotFoundException($"Could not locate file: {binaryPath}");

            using (RawFile binary = new RawFile(binaryPath))
            {
                RenderMesh mesh = DecodeMesh(binary, header, mainNode, originalMesh, scene);

                Console.WriteLine();

                return mesh;
            }

        }

        private static RenderMesh DecodeMesh(RawFile binary, glTFHeader header, Node node, RenderMesh originalMesh, EditorScene scene)
        {
            var glmesh = header.Meshes[node.Mesh ?? -1];
            GltfSkin glskin = null;
            if (node.Skin != null)
                glskin = header.Skins[node.Skin.Value];

            List<glTFPrimitive> primitives = new();

            foreach (var prim in glmesh.Primitives)
            {
                glTFPrimitive primitive = new glTFPrimitive();

                primitive.Indices = glTFBinary.GetIndices(binary, header, prim.Indices);

                if (prim.Mode != 4)
                    throw new NotSupportedException("Unsupported primitive mode, only triangles supported!");

                foreach (var attribute in prim.Attributes)
                {
                    string semantic = attribute.Key;
                    int accessorIndex = attribute.Value;

                    switch (semantic)
                    {
                        case "POSITION":
                            primitive.Positions = glTFBinary.GetFromAccessor<Vector3>(binary, header, accessorIndex);
                            break;
                        case "NORMAL":
                            primitive.Normals = glTFBinary.GetFromAccessor<Vector3>(binary, header, accessorIndex);
                            break;
                        case "COLOR_0":
                            primitive.ColorSet0 = glTFBinary.GetFromAccessor<Vector4>(binary, header, accessorIndex);
                            break;
                        case "COLOR_1":
                            primitive.ColorSet1 = glTFBinary.GetFromAccessor<Vector4>(binary, header, accessorIndex);
                            break;
                        case "JOINTS_0":
                            primitive.Joints = glTFBinary.GetFromAccessor<VectorI4>(binary, header, accessorIndex);
                            break;
                        case "WEIGHTS_0":
                            primitive.Weights = glTFBinary.GetFromAccessor<Vector4>(binary, header, accessorIndex);
                            break;
                        case "TANGENT":
                            primitive.Tangents = glTFBinary.GetFromAccessor<Vector3>(binary, header, accessorIndex);
                            break;
                        default:
                            if (semantic.StartsWith("TEXCOORD_"))
                            {
                                int set = int.Parse(semantic[9..]);

                                primitive.SetUVSet(set, glTFBinary.GetFromAccessor<Vector2>(binary, header, accessorIndex));
                            }
                            else
                            {
                                throw new NotSupportedException($"Unsupported vertex attribute value: {semantic}");
                            }
                            break;

                    }
                }

                primitives.Add(primitive);
            }

            List<Vertex> vertices = new List<Vertex>();

            List<ushort> indices = new List<ushort>();

            int totalVertexCount = 0;
            foreach (var primitive in primitives)
            {
                int thisVertexCount = primitive.Validate();
                for (int i = 0; i < thisVertexCount; i++)
                {
                    vertices.Add(primitive.GetAsVertex(i));
                }

                for (int i = 0; i < primitive.Indices.Count; i++)
                {
                    uint index =
                        primitive.Indices[i] +
                        (uint)totalVertexCount;

                    if (index > ushort.MaxValue)
                    {
                        throw new InvalidDataException(
                            $"Mesh contains vertex index {index}, " + "which exceeds the target format's 16-bit index limit. Reduce the mesh complexity or split the mesh into two");
                    }

                    indices.Add((ushort)index);
                }

                totalVertexCount += thisVertexCount;
            }

            var nuMesh = originalMesh.OriginalMesh;
            if (glskin != null)
            {
                byte counter = 0;
                Dictionary<ushort, byte> usedJoints = new();
                foreach (var vertex in vertices)
                {
                    VectorI4 joints = vertex.BlendIndices;
                    Vector4 weights = vertex.BlendWeights;
                    if (weights.X > 0)
                    {
                        if (!usedJoints.ContainsKey(joints.X))
                            usedJoints.Add(joints.X, counter++);
                        vertex.BlendIndices.X = usedJoints[joints.X];
                    }
                    else
                    {
                        vertex.BlendIndices.X = 0;
                    }
                    if (weights.Y > 0)
                    {
                        if (!usedJoints.ContainsKey(joints.Y))
                            usedJoints.Add(joints.Y, counter++);
                        vertex.BlendIndices.Y = usedJoints[joints.Y];
                    }
                    else
                    {
                        vertex.BlendIndices.Y = 0;
                    }
                    if (weights.Z > 0)
                    {
                        if (!usedJoints.ContainsKey(joints.Z))
                            usedJoints.Add(joints.Z, counter++);
                        vertex.BlendIndices.Z = usedJoints[joints.Z];
                    }
                    else
                    {
                        vertex.BlendIndices.Z = 0;
                    }
                    if (weights.W > 0)
                    {
                        if (!usedJoints.ContainsKey(joints.W))
                            usedJoints.Add(joints.W, counter++);
                        vertex.BlendIndices.W = usedJoints[joints.W];
                    }
                    else
                    {
                        vertex.BlendIndices.W = 0;
                    }
                }
                byte[] remap = new byte[counter];

                foreach (var jointPair in usedJoints)
                {
                    int nodeIndex = glskin.Joints[jointPair.Key];
                    Node gltfJoint = header.Nodes[nodeIndex];

                    int nuJointIndex = -1;
                    for (int i = 0; i < scene.AllJoints.Count; i++)
                    {
                        if (gltfJoint.Name == scene.AllJoints[i].Name)
                        {
                            var eJoint = (EditorJoint)scene.AllJoints[i];
                            nuJointIndex = i;
                            break;
                        }
                    }

                    if (nuJointIndex == -1)
                        throw new Exception($"Could not find glTF bone {gltfJoint.Name} within the scene!");

                    remap[jointPair.Value] = (byte)nuJointIndex;
                }

                int originalMapSize = nuMesh.SkinMtxMap.Count; // doesn't seem to write the correct bones?
                for (int i = 0; i < originalMapSize; i++)
                {
                    if (i < remap.Length)
                        nuMesh.SkinMtxMap[i] = remap[i];
                    else
                        nuMesh.SkinMtxMap[i] = 0xff;
                }
            }

            VertexList[] vertexLists = new VertexList[originalMesh.VertexBuffers.Length];
            RenderVertexBuffer[] vertexBuffers = new RenderVertexBuffer[vertexLists.Length];
            for (int i = 0; i < vertexLists.Length; i++)
            {
                var vertexList = VertexList.FromVertices(vertices, originalMesh.VertexBuffers[i].Attributes);
                var vBuffer = (RenderVertexBuffer)scene.GetOrAdd(RenderVertexBuffer.FromBuffer(vertexList));
                vertexLists[i] = vBuffer.Original;
                vertexBuffers[i] = vBuffer;
            }

            RenderIndicesBuffer indicesBuffer = (RenderIndicesBuffer)scene.GetOrAdd(RenderIndicesBuffer.FromBuffer(indices.ToArray()));

            RenderMesh mesh = new RenderMesh(vertexBuffers, indicesBuffer);
            mesh.IndicesBase = 0;
            mesh.IndicesCount = indices.Count;
            mesh.VerticesBase = 0;
            mesh.VerticesCount = vertices.Count;

            
            nuMesh.VertexBuffers = vertexLists;
            nuMesh.Indices = indicesBuffer.Indices;
            nuMesh.IndicesBase = 0;
            nuMesh.IndicesCount = (uint)indices.Count;
            nuMesh.VerticesBase = 0;
            nuMesh.VerticesCount = (uint)vertices.Count;

            for (int i = 0; i < vertexLists.Length; i++)
            { // fixes a vertex explosion
                nuMesh.VertexBufferFlags[i] = 0x502;
                nuMesh.VertexBufferOffsets[i] = 0;
            }

            nuMesh.IndicesFlags = 0x102;

            mesh.OriginalMesh = nuMesh;

            return mesh;
        }

        //private static T ReadAccessor<T>(RawFile binary, )
        //{

        //}

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
