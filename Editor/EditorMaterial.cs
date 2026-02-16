using Diorama.Core.Filetypes.GSC.Components;
using Diorama.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Mathematics;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Editor
{
    public class EditorMaterial
    {
        public NuMaterialData Original;

        public RenderTexture Diffuse0;
        public RenderTexture Diffuse1;

        public RenderTexture Normal0;

        public int LightmapUVSet = -1;

        public Vector4 Colour1;
    }
}
