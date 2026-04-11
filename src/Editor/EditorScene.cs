using Diorama.Core.Filetypes.GSC;
using Diorama.Core.Filetypes.GSC.Components;
using Diorama.Editor.Metadata;
using Diorama.Rendering;
using Diorama.Rendering.Shaders;
using OpenTK.Graphics.OpenGL4;
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
        public GScene OriginalScene;

        public Matrix4 SceneTransform;

        public string Name { get; set; }

        public EditorMetadata Metadata { get; set; }

        public List<RenderTexture> Textures;
        public List<EditorMaterial> Materials;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<EditorSceneObject> Objects { get; }

        private readonly Dictionary<RenderBuffer, RenderBuffer> _buffers = new();

        /// <summary>
        /// Returns an existing equivalent buffer if found, otherwise adds and returns the new one.
        /// </summary>
        public RenderBuffer GetOrAdd(RenderBuffer buffer)
        {
            if (_buffers.TryGetValue(buffer, out var existing))
            {
                Console.WriteLine("Reusing instance");
                return existing; // reuse existing instance
            }

            buffer.Finalise();
            _buffers[buffer] = buffer;
            return buffer; // new instance
        }


        public EditorScene()
        {
            Textures = new();
            Materials = new();
            Objects = new();
        }

        public void Draw(Shader shader, Camera camera)
        {
            foreach (var obj in Objects)
            {
                obj.Draw(shader, camera);
            }

            GL.BindVertexArray(0);
        }

        public void DebugDraw(Shader shader, Camera camera)
        {
            foreach (var obj in Objects)
            {
                obj.Debug_Draw(shader, camera);
            }
        }
    }
}
