using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Platform.Storage;
using Diorama.Editor;
using Diorama.Rendering;
using Diorama.UI.ViewModels;

namespace Diorama;

public class GeometryObjectPanel : TemplatedControl
{
    public GeometryObjectPanel(SceneController controller)
    {
        DataContext = new GeometryObjectPanelViewModel(controller);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        var button = e.NameScope.Find<Button>("ReplaceMesh");

        if (button != null)
        {
            button.Click += ReplaceMeshClick;
        }
    }

    private async void ReplaceMeshClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var window = TopLevel.GetTopLevel(this) as Window;

        var files = await window.StorageProvider.OpenFilePickerAsync(
            new FilePickerOpenOptions
            {
                Title = "Replace Mesh",
                AllowMultiple = false
            });

        var path = files.FirstOrDefault()?.Path.LocalPath;

        if (DataContext is GeometryObjectPanelViewModel vm)
        {
            vm.ReplaceMesh(path);
        }
    }
}