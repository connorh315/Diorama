using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.Rendering
{
    public interface IRenderer
    {
        bool ContinuousRendering { get; }

        void Initialize();

        void Render(RenderSurface surface);
    }
}
