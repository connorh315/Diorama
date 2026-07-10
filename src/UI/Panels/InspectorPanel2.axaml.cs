using Avalonia.Controls.Primitives;
using Diorama.Rendering;
using System.Windows.Input;

namespace Diorama;

public class InspectorPanel2 : TemplatedControl
{
    public InspectorPanel2()
    {

    }

    public InspectorPanel2(SceneController sceneController)
    {
        DataContext = sceneController;
    }
}