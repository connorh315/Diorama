using Avalonia.Controls.Primitives;
using Diorama.Rendering;
using System.Windows.Input;

namespace Diorama.UI.Controls;

public class SceneHierarchy : TemplatedControl
{
    public SceneHierarchy(SceneController controller)
    {
        DataContext = controller;
    }
}