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

        var replaceMeshButton = e.NameScope.Find<Button>("ReplaceMesh");
        if (replaceMeshButton != null)
            replaceMeshButton.Click += ReplaceMeshClick;

        var exportMeshButton = e.NameScope.Find<Button>("ExportMesh");
        if (exportMeshButton != null)
            exportMeshButton.Click += ExportMeshClick;
    }

    private async void ReplaceMeshClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var window = TopLevel.GetTopLevel(this) as Window;

        var files = await window.StorageProvider.OpenFilePickerAsync(
            new FilePickerOpenOptions
            {
                Title = "Replace Mesh",
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("OBJ files") { Patterns = new[] { "*.OBJ" } }
                }
            });

        var path = files.FirstOrDefault()?.Path.LocalPath;

        if (DataContext is GeometryObjectPanelViewModel vm)
        {
            vm.ReplaceMesh(path);
        }
    }

    private async void ExportMeshClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var window = TopLevel.GetTopLevel(this) as Window;

        var obj = new FilePickerFileType("OBJ file") { Patterns = new[] { "*.OBJ" } };

        var file = await window.StorageProvider.SaveFilePickerAsync(
            new FilePickerSaveOptions
            {
                Title = "Export Mesh",
                FileTypeChoices = new[] { obj },
                SuggestedFileType = obj,
            });

        var path = file?.Path.LocalPath;

        if (DataContext is GeometryObjectPanelViewModel vm)
        {
            vm.ExportMesh(path);
        }
    }
}