using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Utils;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using Diorama.Rendering;
using Diorama.UI.ViewModels;

namespace Diorama;

public class GeometryInspector : TemplatedControl
{
    private InspectorPanelViewModel viewmodel;

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

        InspectorPanel ancestor = this.FindAncestorOfType<InspectorPanel>();

        if (ancestor != null && ancestor.DataContext is InspectorPanelViewModel vm)
        {
            viewmodel = vm;
        }
    }

    private async void DebugMeshClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        viewmodel.DebugMesh();
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
                    new FilePickerFileType("OBJ files") { Patterns = new[] { "*.OBJ" } },
                    new FilePickerFileType("glTF files") { Patterns = new[] { "*.glTF" } }
                }
            });

        var path = files.FirstOrDefault()?.Path.LocalPath;

        viewmodel.ReplaceMesh(path);
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

        viewmodel.ExportMesh(path);
    }
}