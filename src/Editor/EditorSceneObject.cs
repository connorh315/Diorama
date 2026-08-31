using Avalonia.Controls;
using Diorama.Core.Filetypes.GSC.Components;
using Diorama.Rendering;
using Diorama.Rendering.Shaders;
using Diorama.UI.Controls;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Editor
{
    public class EditorSceneObject : EditableItem, INotifyPropertyChanged, IHierarchySelectable, INamedItem
    {
        private string name;
        public string? Name
        {
            get
            {
                if (SpecialObject != null)
                {
                    return SpecialObject.Name;
                }
                else
                {
                    return name;
                }
            }
            set
            {
                if (SpecialObject != null)
                {
                    SpecialObject.Name = value;
                }
                else
                {
                    name = value;
                }

                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        public string DisplayName
        {
            get
            {
                if (SpecialObject != null && SpecialObject.LODGroup != -1)
                {
                    return $"{Name} (LOD {SpecialObject.LODGroup})";
                }

                return Name;
            }
        }

        public bool CanEditName { get => SpecialObject != null; }

        public EditorClipObject ClipObject { get; set; }

        public EditorLodGroup[] Lods { get; set; }

        private EditorSpecialObject? specialObject;
        public EditorSpecialObject? SpecialObject 
        { 
            get => specialObject; 
            set
            {
                if (specialObject == value)
                    return;

                if (specialObject != null)
                    specialObject.PropertyChanged -= SpecialObject_PropertyChanged;

                specialObject = value;

                if (specialObject != null)
                    specialObject.PropertyChanged += SpecialObject_PropertyChanged;

                OnPropertyChanged(nameof(SpecialObject));
                OnPropertyChanged(nameof(SpecialObjectExists));
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(DisplayName));
            } 
        }

        public bool SpecialObjectExists => SpecialObject != null;

        private void SpecialObject_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SpecialObject.Name))
            {
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        public IEnumerable<IHierarchySelectable>? Children =>
            UseLodGroups
                ? Lods
                : ClipObject?.Elements?.Cast<IHierarchySelectable>();

        public bool UseLodGroups { get; set; }

        public float[] FadeDistances { get; set; }

        public float ApproxSize { get; set; }

        public Vector4 BoundsCenterAndDistSqrd { get; set; }

        public Vector4 BoundsExtentsAndRadius { get; set; }

        public bool IsActive = true;

        public bool DebugDraw = false;

        public void Draw(Shader shader, RenderContext ctx)
        {
            if (!IsActive)
                return;

            if (ViewportNewControl.UseFrustumCulling && !ctx.Intersects(BoundsCenterAndDistSqrd.Xyz, BoundsExtentsAndRadius.Xyz))
                return;


            GetActiveClipObject(ctx.CameraScenePosition)?.Draw(shader, ctx);
        }

        public EditorClipObject? GetActiveClipObject(Vector3 cameraPos)
        {
            if (UseLodGroups)
            {
                int result = -1;

                for (int i = 0; i < Lods.Length; i++)
                {
                    if (Vector3.Distance(BoundsCenterAndDistSqrd.Xyz, cameraPos) > Lods[i].FadeDistance && result == -1)
                    {
                        result = i;
                        Lods[i].IsActive = true;
                    }
                    else
                    {
                        Lods[i].IsActive = false;
                    }
                }

                if (result == -1) return null;

                return Lods[result]?.ClipObject;
            }
            else
            {
                return ClipObject;
            }
        }

        public void AddRenderables(RenderContext ctx)
        {
            if (!IsActive)
                return;

            if (ViewportNewControl.UseFrustumCulling && !ctx.Intersects(BoundsCenterAndDistSqrd.Xyz, BoundsExtentsAndRadius.Xyz))
                return;


            GetActiveClipObject(ctx.CameraScenePosition)?.AddRenderables(ctx);
        }

        public void Debug_Draw(Shader shader, Camera camera)
        {
            if (!Name.Contains("LOD")) return;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
