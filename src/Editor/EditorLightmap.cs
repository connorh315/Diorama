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

        public RenderTexture AmbientOcclusion;

        public float[] Offsets = new float[2];
        public float[] Scales = new float[2];
    }
}
