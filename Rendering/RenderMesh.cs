using Diorama.Core.Filetypes.GSC.Components;
using Diorama.Rendering.Shaders;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Rendering
{
    public class RenderMesh
    {
        private int VAO;

        public int VerticesBase;
        public int VerticesCount { get; set; }
        public int IndicesBase;
        public int IndicesCount;

        private VertexAttribPointerType GetType(VertexDefinitionStorageEnum type)
        {
            return type switch
            {
                VertexDefinitionStorageEnum.vec2float => VertexAttribPointerType.Float,
                VertexDefinitionStorageEnum.vec3float => VertexAttribPointerType.Float,
                VertexDefinitionStorageEnum.vec4float => VertexAttribPointerType.Float,
                VertexDefinitionStorageEnum.vec2half => VertexAttribPointerType.HalfFloat,
                VertexDefinitionStorageEnum.vec4half => VertexAttribPointerType.HalfFloat,
                VertexDefinitionStorageEnum.vec4char => VertexAttribPointerType.Byte,
                VertexDefinitionStorageEnum.vec4mini => VertexAttribPointerType.UnsignedByte,
                VertexDefinitionStorageEnum.color4char => VertexAttribPointerType.UnsignedByte,
                _ => throw new NotSupportedException($"Unknown storage type: {type}")
            };
        }

        private bool IsNormalized(VertexDefinitionStorageEnum type) => type == VertexDefinitionStorageEnum.vec4mini || type == VertexDefinitionStorageEnum.color4char;

        public RenderMesh(RenderVertexBuffer[] vBuffers, RenderIndicesBuffer iBuffer)
        {
            VAO = GL.GenVertexArray();

            GL.BindVertexArray(VAO);

            iBuffer.Use();

            foreach (var vb in vBuffers)
            {
                vb.Use();

                foreach (var def in vb.Attributes)
                {
                    int location = (int)def.Variable;


                    GL.VertexAttribPointer(
                        location,
                        def.ComponentCount(),
                        GetType(def.Type),
                        IsNormalized(def.Type),
                        vb.Stride,
                        def.Offset);

                    GL.EnableVertexAttribArray(location);
                }
            }

            GL.BindVertexArray(0);
        }

        public void Draw()
        {
            GL.BindVertexArray(VAO);
            GL.DrawElementsBaseVertex(
                PrimitiveType.Triangles,
                IndicesCount,
                DrawElementsType.UnsignedShort,
                IndicesBase * sizeof(ushort),
                VerticesBase);
            //GL.DrawElements(
            //    PrimitiveType.Triangles,
            //    IndicesCount,
            //    DrawElementsType.UnsignedShort,
            //    IndicesBase * sizeof(ushort));
        }
    }
}
