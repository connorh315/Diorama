using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Rendering
{
    public interface IDioramaRenderer
    {
        Camera Camera { get; }

        void SetFramebufferSize(int width, int height);

        void Initialize();

        void Render();

        void Deinitialize();
    }
}
