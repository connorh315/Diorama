using Diorama.Rendering;
using Diorama.Rendering.Shaders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.Editor
{
    public interface IRenderable
    {
        public void Draw(Shader shader, RenderContext ctx);
    }
}
