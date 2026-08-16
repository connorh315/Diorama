using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Diorama.Editor.Material;
using Diorama.Editor.ShaderSystem;
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

        if (EditorShaderSystem.Cache != null)
        {
            ChangeShaderSetWindow modal = new ChangeShaderSetWindow()
            {
                DataContext = vm
            };

            var window = TopLevel.GetTopLevel(this) as Window;

            await modal.ShowDialog(window);
        }
        else
        {
            MessageWindow message = new MessageWindow("Failed to load fingerprints", ["Could not load fingerprints from disk", "Run the Generate Material Fingerprints option in Settings"]);
            var window = TopLevel.GetTopLevel(this) as Window;

            message.ShowDialog(window);
        }
    }
}