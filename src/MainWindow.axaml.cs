using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using BrickVault;
using BrickVault.Types;
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

            RenderOptionsPanel.DataContext = new RenderOptions();

            //Geometry = new InspectorPanel(sceneController);
            //GeometryHost.Content = Geometry;

            Title = $"Diorama - {Settings.BuildVersion} [{Settings.BuildType}] ({Settings.BuildDate})";

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

        private void SaveFile_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
        }

        private void MenuItem_Click_1(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
        }

#if DEBUG
        protected override async void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key != Key.O || e.KeyModifiers != KeyModifiers.Alt) return;

            Dictionary<DATFile, List<ArchiveFile>> archives = new();

            string datLocations = Settings.DatLocation;
            foreach (var dat in Directory.EnumerateFiles(datLocations, "*.DAT", SearchOption.AllDirectories))
            {
                var archive = DATFile.Open(dat);
                if (archive == null) continue;
                archives.Add(archive, new());
                foreach (var file in archive.GetFilesWithExtension("gsc"))
                {
                    archives[archive].Add(file);
                }
            }

            OpenFromArchiveViewModel vm = new OpenFromArchiveViewModel(archives);
            OpenFromArchive modal = new OpenFromArchive()
            {
                DataContext = vm
            };

            await modal.ShowDialog(this);

            using (RawFile scene = new RawFile(new MemoryStream()))
            using (RawFile textures = new RawFile(new MemoryStream()))
            {
                if (vm.Commited && vm.Selected != null)
                {
                    bool hasScene = false;
                    bool hasTextures = false;

                    foreach ((DATFile archive, List<ArchiveFile> files) in archives)
                    {
                        ArchiveFile texturesFile = archive.FileTree.GetFile(Path.ChangeExtension(vm.Selected.Path, "nxg_textures"));

                        using (var ctx = archive.GetExtractionContext())
                        {
                            if (!hasTextures && texturesFile != null)
                            {
                                archive.ExtractFile(texturesFile, ctx, textures.fileStream);

                                textures.Seek(0, SeekOrigin.Begin);

                                hasTextures = true;
                            }

                            if (!hasScene && files.Contains(vm.Selected))
                            {
                                archive.ExtractFile(vm.Selected, ctx, scene.fileStream);

                                scene.Seek(0, SeekOrigin.Begin);

                                hasScene = true;
                            }

                            if (hasTextures && hasScene) break;
                        }
                    }

                    MainViewport.LoadScene(scene, textures, $"dat://{vm.Selected.Path}");
                }
            }
        }
    }
#endif
}