using Diorama.Rendering;
using Diorama.Rendering.Shaders;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Editor
{
    public class EditorScene
    {
        public Matrix4 SceneTransform;

        public List<RenderTexture> Textures;
        public List<EditorMaterial> Materials;
        public List<EditorSceneObject> Objects;

        public EditorScene()
        {
            Textures = new();
            Materials = new();
            Objects = new();
        }

        public void Draw(Shader shader)
        {
            foreach (var obj in Objects)
            {
                obj.Draw(shader);
            }
        }
    }
}
