using Avalonia.Media.TextFormatting;
using Diorama.Core.Filetypes.GSC;
using Diorama.Core.Filetypes.GSC.Components;
using Diorama.Core.Filetypes.TEXTURES;
using Diorama.Editor;
using Diorama.Extensions;
using Diorama.Rendering.Shaders;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Rendering
{
    public class RenderScene
    {
        public List<RenderMesh> Meshes;
        public List<RenderTexture> Textures;

        public Matrix4 SceneTransform = Matrix4.Identity;

        public void Draw(Camera camera, Shader shader)
        {
            shader.SetMatrix4("projection", camera.Projection);
            shader.SetMatrix4("view", SceneTransform * camera.GetViewMatrix());

            //foreach (RenderMesh mesh in Meshes)
            //{
            //    if (mesh.Diffuse0Index > -1)
            //    {
            //        Textures[mesh.Diffuse0Index].Use();
            //    }
            //    else
            //    {
            //        RenderTexture.GetWhiteTexture().Use();
            //    }

            //    if (mesh.Diffuse1Index > -1)
            //    {
            //        Textures[mesh.Diffuse1Index].Use(TextureUnit.Texture1);
            //    }
            //    else
            //    {
            //        RenderTexture.GetWhiteTexture().Use(TextureUnit.Texture1);
            //    }

            //    mesh.Draw();
            //}
        }

        
    }
}
