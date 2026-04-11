using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Diorama.Rendering;
using Diorama.UI.Controls;

namespace Diorama
{
    public partial class MainWindow : Window
    {
        private SceneController sceneController;
        
        private ViewportNewControl MainViewport;
        private SceneHierarchy Hierarchy;
        private InspectorPanel Inspector;
        private GeometryObjectPanel Geometry;

        public MainWindow()
        {
            InitializeComponent();

            sceneController = new SceneController(new ViewportRenderer(), this);

            MainViewport = new ViewportNewControl(sceneController);
            ViewportHost.Content = MainViewport;

            Hierarchy = new SceneHierarchy(sceneController);
            HierarchyHost.Content = Hierarchy;

            Inspector = new InspectorPanel(sceneController);
            InspectorHost.Content = Inspector;

            Geometry = new GeometryObjectPanel(sceneController);
            GeometryHost.Content = Geometry;

            Title = $"Diorama - {Settings.BuildVersion} [{Settings.BuildType}] ({Settings.BuildDate})";
        }

        private void Window_DragDrop(object sender, DragEventArgs e)
        {
            string firstFile = string.Empty;
            if (e.DataTransfer.Formats.Contains(DataFormat.File))
            {
                var files = e.DataTransfer.TryGetFiles();
                if (files != null)
                {
                    firstFile = files.First().Path.LocalPath;
                }
            }

            if (firstFile == string.Empty) return;

#if DEBUG // When triggering a breakpoint, explorer sort of just freezes until the code continues which is insufferable
                    Dispatcher.UIThread.Invoke(new Action(() =>
            {
#endif
                MainViewport.LoadScene(firstFile);
#if DEBUG
            }), DispatcherPriority.Background);
#endif
        }

        private void LightmapItem_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ViewportNewControl.ShowLightmaps = !ViewportNewControl.ShowLightmaps;
        }

        private void CameraLightItem_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ViewportNewControl.UseCameraLight = !ViewportNewControl.UseCameraLight;
        }

        private void OpenFile_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            OpenFileMenu();
        }

        private async void OpenFileMenu()
        {
            if (StorageProvider == null)
                throw new Exception("Unable to access filesystem");

            var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Open StreamInfo File",
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("GScene files") { Patterns = new[] { "*.GSC", "*.GHG" } }
                }
            });

            if (files.Count > 0)
            {
                string filePath = files[0].Path.LocalPath;

                MainViewport.LoadScene(filePath);
            }
        }

        private void SaveFile_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
        }

        private void MenuItem_Click_1(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
        }
    }
}