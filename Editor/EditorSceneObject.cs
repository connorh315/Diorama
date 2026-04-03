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
    public class EditorSceneObject : INotifyPropertyChanged, IHierarchySelectable
    {
        private string name;
        public string Name
        {
            get => name;
            set
            {
                if (name == value) return;
                name = value;
                OnPropertyChanged(nameof(Name));

                if (SpecialObject != null)
                {
                    SpecialObject.Name = value;
                }
            }
        }

        public bool CanEditName { get => SpecialObject != null; }

        public EditorClipObject ClipObject { get; set; }

        public EditorLodGroup[] Lods { get; set; }

        public NuSpecialObject? SpecialObject { get; set; }

        public IEnumerable<IHierarchySelectable>? Children =>
            UseLodGroups
                ? Lods
                : ClipObject?.Elements?.Cast<IHierarchySelectable>();

        public bool UseLodGroups { get; set; }

        public float[] FadeDistances { get; set; }

        public Vector4 BoundsCenterAndDistSqrd { get; set; }

        public bool IsActive = true;

        public bool DebugDraw = false;

        public void Draw(Shader shader, Camera camera)
        {
            if (!IsActive)
                return;

            GetActiveClipObject(camera)?.Draw(shader);
        }

        public EditorClipObject? GetActiveClipObject(Camera camera)
        {
            if (UseLodGroups)
            {
                int result = -1;

                for (int i = 0; i < Lods.Length; i++)
                {
                    Lods[i].IsActive = false;
                    
                    if (result != -1) continue;

                    if (Vector3.Distance(BoundsCenterAndDistSqrd.Xyz, camera.Position) > Lods[i].FadeDistance)
                    {
                        result = i;
                        Lods[i].IsActive = true;
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

        public void Debug_Draw(Shader shader, Camera camera)
        {
            if (!Name.Contains("LOD")) return;

            //shader.SetMatrix4("model", Transform);

            //if (Vector3.DistanceSquared(BoundsCenterAndDistSqrd.Xyz, camera.Position) > BoundsCenterAndDistSqrd.W)
            //{
            //    shader.SetVector4("mesh_color", new Vector4(1, 0, 0, 1));
            //    RenderTexture.GetWhiteTexture().Use(TextureUnit.Texture0);
            //    RenderTexture.GetWhiteTexture().Use(TextureUnit.Texture1);
            //}
            //else
            //{
            //    shader.SetVector4("mesh_color", Material.Colour1);
            //    Material.Diffuse0?.Use();
            //    Material.Diffuse1?.Use(TextureUnit.Texture1);
            //}

            //if (Lightmap != null && Lightmap.AmbientOcclusion != null && ViewportNewControl.ShowLightmaps && Material.LightmapUVSet != -1)
            //{
            //    Lightmap.AmbientOcclusion.Use(TextureUnit.Texture2);
            //    shader.SetVector2("lm_offset", new Vector2(Lightmap.Offsets[0], Lightmap.Offsets[1]));
            //    shader.SetVector2("lm_scale", new Vector2(Lightmap.Scales[0], Lightmap.Scales[1]));
            //    shader.SetInt("lightmap_uvset", Material.LightmapUVSet);
            //}
            //else
            //{
            //    RenderTexture.GetWhiteTexture().Use(TextureUnit.Texture2);
            //}

            //Mesh.Draw();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
