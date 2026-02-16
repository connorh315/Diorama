using Avalonia.Controls.Primitives;
using Diorama.Rendering;

namespace Diorama;

public class InspectorPanel : TemplatedControl
{
    public InspectorPanel(SceneController sceneController)
    {
        DataContext = sceneController;
    }
}