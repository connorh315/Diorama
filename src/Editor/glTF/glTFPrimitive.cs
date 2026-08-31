using Diorama.Core.Filetypes.GSC.Components;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Diorama.Editor.glTF
{
    public class glTFPrimitive
    {
        public List<Vector3>? Positions { get; set; }
        public List<Vector3>? Normals { get; set; }
        public List<Vector3>? Tangents { get; set; }

        public Dictionary<int, List<Vector2>> UVSets { get; } = new();
        
        public void SetUVSet(int index, List<Vector2> uvSet) => UVSets[index] = uvSet;

        public List<Vector2> GetUVSet(int index) => UVSets[index];

        public int UVSetCount => UVSets.Count;

        public List<Vector4>? ColorSet0 { get; set; }
        public List<Vector4>? ColorSet1 { get; set; }

        public List<VectorI4> Joints { get; set; }
        public List<Vector4> Weights { get; set; }

        public List<ushort> Indices { get; set; }

        public int Validate()
        {
            int vertices = Positions.Count;
            if (Normals != null && vertices != Normals.Count)
                throw new InvalidDataException($"Vertex attributes do not align!");

            return vertices;
        }

        public Vertex GetAsVertex(int index)
        {
            Vertex vertex = new Vertex();
            vertex.Position = Positions[index];
            if (Normals != null)
                vertex.Normal = Normals[index];
            if (Tangents != null)
                vertex.Tangent = Tangents[index];
            if (ColorSet0 != null)
                vertex.ColorSet0 = ColorSet0[index];
            if (ColorSet1 != null)
                vertex.ColorSet1 = ColorSet1[index];
            if (Joints != null)
                vertex.BlendIndices = Joints[index];
            if (Weights != null)
                vertex.BlendWeights = Weights[index];
            
            int uvsets = UVSetCount;

            if (uvsets == 1)
                vertex.UVSet01 = new Vector4(GetUVSet(0)[index], 0, 0);
            else if (uvsets == 2)
            {
                Vector2 xy = GetUVSet(0)[index];
                Vector2 zw = GetUVSet(1)[index];
                vertex.UVSet01 = CombineVector2(xy, zw);
            }
            else if (uvsets == 3)
            {
                Vector2 xy0 = GetUVSet(0)[index];
                Vector2 zw0 = GetUVSet(1)[index];
                vertex.UVSet01 = CombineVector2(xy0, zw0);
                Vector2 xy1 = GetUVSet(2)[index];

                vertex.UVSet23 = new Vector4(xy1, 0, 0);
            }
            else if (uvsets == 4)
            {
                Vector2 xy0 = GetUVSet(0)[index];
                Vector2 zw0 = GetUVSet(1)[index];
                vertex.UVSet01 = CombineVector2(xy0, zw0);
                Vector2 xy1 = GetUVSet(2)[index];
                Vector2 zw1 = GetUVSet(3)[index];
                vertex.UVSet23 = CombineVector2(xy1, zw1);
            }

            return vertex;
        }

        private Vector4 CombineVector2(Vector2 xy, Vector2 zw) => new Vector4(xy, zw.X, zw.Y);
    }
}
