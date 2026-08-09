using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Diorama.Editor.glTF
{
    public class glTFHeader
    {
        public Asset Asset { get; set; }
        public int Scene { get; set; }
        public List<Scene> Scenes { get; set; }
        public List<Node> Nodes { get; set; }
        public List<GltfMesh> Meshes { get; set; }
        public List<GltfAccessor> Accessors { get; set; }
        public List<GltfBufferView> BufferViews { get; set; }
        public List<GltfBuffer> Buffers { get; set; }
        public List<GltfSkin> Skins { get; set; }

        public glTFHeader()
        {
            Asset = new Asset() { Generator = $"{AppSettings.AppString} glTF exporter" };
            Scene = 0;
            Scenes = new();
            Nodes = new();
            Meshes = new();
            Accessors = new();
            BufferViews = new();
            Buffers = new();
            Skins = new();
        }

        public int AddScene()
        {
            Scenes.Add(new Scene { Nodes = [] });
            return Scenes.Count - 1;
        }

        public Node AddMeshNodeToScene(int sceneIndex, string nodeName)
        {
            var node = new Node { Name = nodeName, Mesh = Meshes.Count };
            Scenes[sceneIndex].Nodes.Add(Nodes.Count);
            Nodes.Add(node);
            return node;
        }

        public int AddMesh(string meshName, List<GltfPrimitive> primitives)
        {
            Meshes.Add(new GltfMesh { Name = meshName, Primitives = primitives });
            return Meshes.Count - 1;
        }

        public int AddBuffer(string bufferPath, int bufferLength)
        {
            Buffers.Add(new GltfBuffer
            {
                Uri = bufferPath,
                ByteLength = bufferLength
            });
            return Buffers.Count - 1;
        }

        public int AddBufferView(GltfBufferView view)
        {
            BufferViews.Add(view);
            return BufferViews.Count - 1;
        }

        public int AddAccessor(GltfAccessor accessor)
        {
            Accessors.Add(accessor);
            return Accessors.Count - 1;
        }

        public int AddSkin(GltfSkin skin)
        {
            Skins.Add(skin);
            return Skins.Count - 1;
        }

        public void WriteToFile(string filePath)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            string json = JsonSerializer.Serialize(this, options);

            File.WriteAllText(filePath, json);
        }
    }

    public class Asset
    {
        public string Version { get; set; } = "2.0";
        public string Generator { get; set; }
    }

    public class Scene
    {
        public List<int> Nodes { get; set; } = new();
    }

    public class Node
    {
        public string Name { get; set; }
        public int? Mesh { get; set; }
        public int? Skin { get; set; }
        public List<int>? Children { get; set; }
        public List<float>? Matrix { get; set; }
    }

    public class GltfMesh
    {
        public string Name { get; set; }
        public List<GltfPrimitive> Primitives { get; set; }
    }

    public class GltfPrimitive
    {
        public Dictionary<string, int> Attributes { get; set; } = new();

        public int Indices { get; set; }

        public int Mode { get; set; } = 4; // triangles
    }

    public class GltfAccessor
    {
        public int BufferView { get; set; }
        public int ComponentType { get; set; }
        public int Count { get; set; }
        public string Type { get; set; } = "";
        public float[]? Min { get; set; }
        public float[]? Max { get; set; }
    }

    public class GltfBufferView
    {
        public int Buffer { get; set; }
        public int ByteOffset { get; set; }
        public int ByteLength { get; set; }
        public int? Target { get; set; }
    }

    public class GltfBuffer
    {
        public string Uri { get; set; }
        public int ByteLength { get; set; }
    }

    public enum GltfComponentType
    {
        Byte = 5120,
        UnsignedByte = 5121,
        Short = 5122,
        UnsignedShort = 5123,
        UnsignedInt = 5125,
        Float = 5126
    }

    public enum GltfPrimitiveMode
    {
        Points = 0,
        Lines = 1,
        LineLoop = 2,
        LineStrip = 3,
        Triangles = 4,
        TriangleStrip = 5,
        TriangleFan = 6
    }

    public class GltfSkin
    {
        public string Name { get; set; }
        public int InverseBindMatrices { get; set; }
        public int Skeleton { get; set; }
        public List<int> Joints { get; set; } = new();
    }
}
