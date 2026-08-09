using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Platform.Storage;
using Diorama.Editor;
using Diorama.Rendering;
using Diorama.UI.ViewModels;

namespace Diorama;

public class InspectorPanel : TemplatedControl
{
    public InspectorPanel(SceneController controller)
    {
        DataContext = new InspectorPanelViewModel(controller);
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

        var debugMeshButton = e.NameScope.Find<Button>("DebugMesh");

        if (debugMeshButton != null)
        {
#if DEBUG
            debugMeshButton?.IsVisible = true;
#else
            debugMeshButton?.IsVisible = false;
#endif
            debugMeshButton.Click += DebugMeshClick;
        }

        var rebuildButton = e.NameScope.Find<Button>("RebuildVertexDescriptors");
        if (rebuildButton != null)
            rebuildButton.Click += RebuildButtonClick;
    }

    private async void DebugMeshClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is InspectorPanelViewModel vm)
        {
            vm.DebugMesh();
        }
    }

    private async void RebuildButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is InspectorPanelViewModel vm)
        {
            List<string> problems = vm.RebuildMaterialVertex();
            if (problems == null || problems.Count == 0) return;


            var window = TopLevel.GetTopLevel(this) as Window;

            if (window != null)
            {
                problems.Add("The material has not been changed.");
                var message = new MessageWindow("Could not rebuild material", problems);
                await message.ShowDialog(window);
            }
        }
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

        if (DataContext is InspectorPanelViewModel vm)
        {
            vm.ReplaceMesh(path);
        }
    }

    private async void ExportMeshClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var window = TopLevel.GetTopLevel(this) as Window;

        var obj = new FilePickerFileType("OBJ file") { Patterns = new[] { "*.OBJ" } };
        var gltf = new FilePickerFileType("glTF file") { Patterns = new[] { "*.glTF" } };

        var file = await window.StorageProvider.SaveFilePickerAsync(
            new FilePickerSaveOptions
            {
                Title = "Export Mesh",
                FileTypeChoices = new[] { obj, gltf },
                SuggestedFileType = obj,
            });

        var path = file?.Path.LocalPath;

        if (DataContext is InspectorPanelViewModel vm)
        {
            vm.ExportMesh(path);
        }
    }
}