using Diorama.Core;
using Diorama.Core.Filetypes.GSC.Components;
using Diorama.Editor.Attributes;
using Diorama.Rendering;
using Diorama.Rendering.Shaders;
using Diorama.UI.Controls;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;
using static OpenTK.Graphics.OpenGL.GL;

namespace Diorama.Editor
{
    public class EditorPointOfInterest : EditableItem, INamedItem, IHierarchySelectable, IRenderable
    {
        public NuPointOfInterest Original;

        public EditorScene ParentScene { get; }

        public EditorPointOfInterest(NuPointOfInterest original, EditorScene parentScene)
        {
            Original = original;
            ParentScene = parentScene;
        }

        [DisplayLabel("Name")]
        public string Name { get => Original.Name; set => Set(ref Original.Name, value); }

        [DisplayLabel("Offset from Parent Joint")]
        public Matrix4 Transform { 
            get => Original.Offset.mtx.ToMatrix4(); 
            set => Set(ref Original.Offset.mtx, value.ToArray()); 
        }

        public EditorJoint Parent { get; set; }

        public void Draw(Shader shader, RenderMesh mesh)
        {
            shader.SetMatrix4("model", Matrix4.CreateScale(0.001f) * Transform * Parent.WorldTransform);

            mesh.Draw();
        }

        public void Draw(Shader shader, RenderContext ctx)
        {
            shader.SetMatrix4("model", Matrix4.CreateScale(0.005f * Vector3.Distance(Transform.ExtractTranslation(), ctx.CameraScenePosition)) * Transform * Parent.WorldTransform);

            MeshFactory.GetIcosahedron().Draw();
        }

        public IEnumerable<IHierarchySelectable> Children => Enumerable.Empty<IHierarchySelectable>();
    }
}
