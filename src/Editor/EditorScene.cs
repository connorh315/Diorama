using Diorama.Core.Filetypes.GSC;
using Diorama.Core.Filetypes.GSC.Components;
using Diorama.Core.Filetypes.TEXTURES;
using Diorama.Editor.Material;
using Diorama.Editor.Metadata;
using Diorama.Rendering;
using Diorama.Rendering.Shaders;
using Diorama.UI.Controls;
using Diorama.UI.ViewModels;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OpenTK.Graphics.OpenGL.GL;

namespace Diorama.Editor
{
    public class EditorScene : INotifyPropertyChanged
    {
        public GScene OriginalScene;

        public NxgTextures OriginalTextures;

        public NxgTextures OriginalCubemapTextures;

        public Matrix4 SceneTransform;

        public string Name { get; set; }

        public EditorMetadata Metadata { get; set; }

        public ObservableCollection<RenderTexture> Textures { get; set; }
        public ObservableCollection<RenderTexture> CubemapTextures { get; set; }
        public ObservableCollection<EditorMaterial> Materials { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<INamedItem> Objects { get; }
        public ObservableCollection<INamedItem> Joints { get; }
        public ObservableCollection<INamedItem> AllJoints { get; }
        public ObservableCollection<INamedItem> PoIs { get; }
        public ObservableCollection<INamedItem> Layers { get; }
        public ObservableCollection<INamedItem> SpecialObjects { get; }

        private readonly Dictionary<RenderBuffer, RenderBuffer> _buffers = new();

        public ObservableCollection<EditorHierarchyGroup> HierarchyItems { get; }

        /// <summary>
        /// Returns an existing equivalent buffer if found, otherwise adds and returns the new one.
        /// </summary>
        public RenderBuffer GetOrAdd(RenderBuffer buffer)
        {
            if (_buffers.TryGetValue(buffer, out var existing))
            {
                return existing; // reuse existing instance
            }

            buffer.Finalise();
            _buffers[buffer] = buffer;
            return buffer; // new instance
        }

        public void Add(RenderBuffer buffer)
        {
            buffer.Finalise();
            if (_buffers.ContainsKey(buffer))
                Console.WriteLine("Caution: A duplicate, identical vertex list is defined in the file.");

        }


        public EditorScene()
        {
            Textures = new();
            Materials = new();
            Objects = new();
            Joints = new();
            AllJoints = new();
            PoIs = new();
            Layers = new();
            SpecialObjects = new();

            HierarchyItems = 
            [
                new("Geometry", Objects),
                new("Joints", Joints),
                new("Points of Interest", PoIs),
                new("Layers", Layers)
            ];
        }

        public void AddRenderables(RenderContext ctx)
        {
            if (CubemapTextures.Count > 0)
            {
                CubemapTextures[0].Use(TextureUnit.Texture17);
            }

            foreach (EditorSceneObject obj in Objects)
            {
                obj.AddRenderables(ctx);
            }

            if (RenderOptions.ShowPoIs)
            {
                foreach (EditorPointOfInterest poi in PoIs)
                {
                    ctx.Gizmos.Add(poi);
                }
            }
        }

        public void Draw(Shader shader, RenderContext ctx)
        {
            if (CubemapTextures.Count > 0)
            {
                CubemapTextures[0].Use(TextureUnit.Texture17);
            }

            foreach (EditorSceneObject obj in Objects)
            {
                obj.Draw(shader, ctx);
            }

            GL.BindVertexArray(0);
        }

        public void DrawPoIs(Shader shader, RenderContext ctx)
        {
            RenderMesh icosahedron = MeshFactory.GetIcosahedron();

            foreach (EditorPointOfInterest poi in PoIs)
            {
                poi.Draw(shader, icosahedron);
            }
        }

        public void DebugDraw(Shader shader, Camera camera)
        {
            foreach (EditorSceneObject obj in Objects)
            {
                obj.Debug_Draw(shader, camera);
            }
        }
    }
}
