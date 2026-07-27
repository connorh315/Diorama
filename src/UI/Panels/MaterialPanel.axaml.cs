using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Diorama.Editor;
using Diorama.Rendering;
using Diorama.UI.Panels;
using Diorama.UI.ViewModels;

namespace Diorama;

public partial class MaterialPanel : TexturesBasePanel
{
    public MaterialPanel()
    {
        InitializeComponent();
    }

    private async void SubstituteShaderSet_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        ChangeShaderSetViewModel vm = new ChangeShaderSetViewModel((EditorMaterial)DataContext);

        ChangeShaderSetWindow modal = new ChangeShaderSetWindow()
        {
            DataContext = vm
        };

        var window = TopLevel.GetTopLevel(this) as Window;

        await modal.ShowDialog(window);
    }
}