using Diorama.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Rendering
{
    public interface IDioramaRenderer
    {
        void SetFramebufferSize(int width, int height);

        void Initialize();

        void Render(List<EditorScene> scenes, Camera camera);

        void Deinitialize();
    }
}
