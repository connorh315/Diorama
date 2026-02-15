using Diorama.Rendering;
using Diorama.Rendering.Shaders;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Editor
{
    public class EditorSceneObject
    {
        public string Name;
        public Matrix4 Transform;
        public EditorMaterial Material;
        public RenderMesh Mesh;

        public void Draw(Shader shader)
        {
            shader.SetMatrix4("model", Transform);

            Material.Diffuse0?.Use();
            Material.Diffuse1?.Use(TextureUnit.Texture1);

            Mesh.Draw();
        }
    }
}
