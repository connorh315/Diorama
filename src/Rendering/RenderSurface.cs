using Diorama.UI.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.Rendering
{
    public class RenderSurface
    {
        public required GlHost Host { get; init; }

        public required IRenderer Renderer { get; init; }

        public bool IsDirty { get; set; } = true;

        internal bool Initialized { get; set; }
    }
}
