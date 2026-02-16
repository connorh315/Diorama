using Diorama.Rendering;
using Diorama.Rendering.Shaders;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Editor
{
    public class EditorScene : INotifyPropertyChanged
    {
        public Matrix4 SceneTransform;

        public string Name { get; set; }

        public List<RenderTexture> Textures;
        public List<EditorMaterial> Materials;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<EditorSceneObject> Objects { get; }

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
