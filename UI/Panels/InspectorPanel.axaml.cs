using Avalonia.Controls.Primitives;
using Diorama.Rendering;
using System.Windows.Input;

namespace Diorama;

public class InspectorPanel : TemplatedControl
{
    public InspectorPanel()
    {

    }

    public InspectorPanel(SceneController sceneController)
    {
        DataContext = sceneController;
    }
}