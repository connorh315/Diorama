using Diorama.Core.Filetypes.GSC.Components;
using Diorama.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Editor
{
    public class EditorLightmap
    {
        public NuLightmapData Original;

        public RenderTexture AmbientOcclusion { get; set; }

        public RenderTexture Smooth { get; set; }

        public RenderTexture Directional0 { get; set; }
        public RenderTexture Directional1 { get; set; }
        public RenderTexture Directional2 { get; set; }

        public float[] Offsets = new float[2];
        public float[] Scales = new float[2];
    }
}
