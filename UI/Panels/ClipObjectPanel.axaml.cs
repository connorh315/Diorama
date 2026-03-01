using Avalonia;
using Avalonia.Controls.Primitives;
using Diorama.Editor;
using Diorama.Rendering;

namespace Diorama;

public class ClipObjectPanel : TemplatedControl
{
    public ClipObjectPanel(SceneController controller)
    {
        DataContext = controller;
    }
}