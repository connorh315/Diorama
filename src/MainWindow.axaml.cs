using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using BrickVault;
using BrickVault.Types;
using Diorama.Core.Filetypes.TEXTURES;
using Diorama.Core.IO;
using Diorama.Rendering;
using Diorama.UI.Controls;
using Diorama.UI.ViewModels;

namespace Diorama
{
    public partial class MainWindow : Window
    {
        private SceneController sceneController;
        
        private ViewportNewControl MainViewport;
        private SceneHierarchy Hierarchy;
        private InspectorPanel Inspector;
        //private InspectorPanel Geometry;

        public MainWindow()
        {
            InitializeComponent();

            AppSettings.Initialize();

            var renderService = new RenderService();

            var viewportRenderer = new ViewportRenderer();
            sceneController = new SceneController(viewportRenderer, this);
            viewportRenderer.Controller = sceneController;

            MainViewport = new ViewportNewControl(sceneController);
            ViewportHost.Content = MainViewport;

            Hierarchy = new SceneHierarchy(sceneController);
            HierarchyHost.Content = Hierarchy;

            //InspectorHost.Content = new TexturePreviewControl(RenderService, new TextureRenderer());

            Inspector = new InspectorPanel(sceneController);
            InspectorHost.Content = Inspector;

            RenderOptionsItems.DataContext = new RenderOptions();

            //Geometry = new InspectorPanel(sceneController);
            //GeometryHost.Content = Geometry;

            Title = $"Diorama - {AppSettings.BuildVersion} [{AppSettings.BuildType}] ({AppSettings.BuildDate})";

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                if (!File.Exists(args[1]))
                {
                    Console.WriteLine("Invalid file path provided for scene");
                    return;
                }
                MainViewport.LoadScene(args[1]);
            }

            this.AttachDevTools();
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

        private void ShadowItem_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ViewportNewControl.ShowShadowImpostors = !ViewportNewControl.ShowShadowImpostors;
        }

        private void CullingItem_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ViewportNewControl.UseFrustumCulling = !ViewportNewControl.UseFrustumCulling;
        }

        private async void Settings_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            SettingsWindow settings = new SettingsWindow();

            await settings.ShowDialog(this);
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
                Title = "Open GScene File",
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

        private void OpenArchiveFile_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            OpenArchiveFile();
        }

        private async void OpenArchiveFile()
        {
            List<FileLocation> filePaths = new();

            foreach (var fileLocation in FileProvider.EnumerateLocations("gsc", "ghg"))
            {
                filePaths.Add(fileLocation);
            }

            OpenFromArchiveViewModel vm = new OpenFromArchiveViewModel(filePaths);
            OpenFromArchive modal = new OpenFromArchive()
            {
                DataContext = vm
            };

            await modal.ShowDialog(this);

            if (vm.Commited && vm.Selected != null)
            {
                var sceneLocation = vm.Selected;
                var texturesLocation = FileProvider.ReplaceLocationExtension(sceneLocation, "nxg_textures");
                var cubemapsLocation = FileProvider.ReplaceInLocation(texturesLocation, "_dx11.nxg_textures", "_cubemaps_dx11.nxg_textures");

                using RawFile scene = FileProvider.GetFile(sceneLocation);
                using RawFile textures = FileProvider.GetFile(texturesLocation);
                using RawFile cubemap_textures = FileProvider.GetFile(cubemapsLocation);

                MainViewport.LoadScene(scene, textures, cubemap_textures, sceneLocation.FullPath);
            }
        }

        public async Task<string?> OpenSaveMenu(string title, string extension)
        {
            if (StorageProvider == null)
                throw new Exception("Unable to access filesystem");

            var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = title,
                DefaultExtension = extension,
                FileTypeChoices = [new FilePickerFileType(extension) { Patterns = [$"*.{extension.TrimStart('.')}"] }]
            });

            return file?.TryGetLocalPath();
        }

        private void SaveFile_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
        }

        private void MenuItem_Click_1(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
        }

        protected override async void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.O && e.KeyModifiers == KeyModifiers.Alt)
            {
                OpenArchiveFile();
            }
            else if (e.Key == Key.O && e.KeyModifiers == KeyModifiers.Control)
            {
                OpenFileMenu();
            }

        }
    }
}