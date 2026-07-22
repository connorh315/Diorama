using Diorama.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.UI.Panels
{
    internal interface ITexturesPanel
    {
        public IEnumerable<RenderTexture> Textures { get; set; }
    }
}
