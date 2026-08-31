using Avalonia.Input;
using Avalonia.Threading;
using Diorama.Core;
using Diorama.Core.Filetypes.GSC;
using Diorama.Core.Filetypes.GSC.Components;
using Diorama.Core.Filetypes.TEXTURES;
using Diorama.Editor;
using Diorama.Editor.glTF;
using Diorama.UI;
using Diorama.UI.ViewModels;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Diorama.Rendering
{
    public class SceneController : INotifyPropertyChanged
    {
        private MainWindow MainWindow;

        public ObservableCollection<EditorScene> Scenes { get; } = new();
        public IRenderer Renderer;

        public EditorSceneObject? SelectedSceneObject =>
            SelectedHierarchyObject switch
            {
                EditorSceneObject obj => obj,
                EditorGeometryObject geo => geo.Parent.Parent,
                _ => null
            };

        public EditorGeometryObject? SelectedGeometry =>
            SelectedHierarchyObject as EditorGeometryObject;

        public ObservableCollection<RenderTexture> SelectedTextures => SelectedGeometry?.Parent.SceneOwner.Textures;

        private IHierarchySelectable? selectedHierarchyObject;
        public IHierarchySelectable? SelectedHierarchyObject
        {
            get => selectedHierarchyObject;
            set
            {
                if (selectedHierarchyObject == value)
                    return;

                selectedHierarchyObject = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(SelectedSceneObject));
                OnPropertyChanged(nameof(SelectedGeometry));
                OnPropertyChanged(nameof(SelectedTextures));
            }
        }
        public ICommand SafeRestructure { get; }
        public ICommand SaveSceneCommand { get; }
        public ICommand RemoveSceneCommand { get; }
        public ICommand ExportSceneCommand { get; }
        public ICommand EditResourceHeaderCommand { get; }
        public ICommand EditTexturesCommand { get; }
        public ICommand SaveTexturesCommand { get; }

        public CameraController CameraController { get; }
        public Camera Camera { get; }

        public SceneController(IRenderer renderer, MainWindow window)
        {
            Renderer = renderer;
            MainWindow = window;

            Camera = new Camera(Vector3.Zero);
            CameraController = new CameraController(Camera);

            SafeRestructure = new RelayCommand<IHierarchySelectable>(async (IHierarchySelectable? sender) =>
            {
                SelectedHierarchyObject = null;
            });

            SaveSceneCommand = new RelayCommand<EditorScene>(async (EditorScene? sender) =>
            {
                string path = sender.OriginalScene.Path;

                bool hasPath = !string.IsNullOrEmpty(path);
                string extension = hasPath ? Path.GetExtension(path) : "gsc";

                if (!hasPath || path.StartsWith("dat:"))
                {
                    string outputPath = await MainWindow?.OpenSaveMenu("Save GScene", extension);

                    if (outputPath == null) return;

                    sender.OriginalScene.Path = outputPath;
                }

                GSceneConverter.Write(sender);
            });

            RemoveSceneCommand = new RelayCommand<EditorScene>((EditorScene? sender) =>
            {
                if (sender != null)
                {
                    Scenes.Remove(sender);
                }
            });

            ExportSceneCommand = new RelayCommand<EditorScene>(async (EditorScene? sender) =>
            {
                string outputPath = await MainWindow?.OpenSaveMenu("Export glTF Scene", "glTF");

                if (outputPath == null) return;

                List<EditorGeometryObject> geometries = new();

                foreach (EditorSceneObject sceneObject in sender.Objects)
                {
                    if (sceneObject.ClipObject != null)
                    {
                        geometries.AddRange(sceneObject.ClipObject.Elements);
                    }
                    else if (sceneObject.UseLodGroups)
                    {
                        if (sceneObject.Lods[0]?.ClipObject != null)
                        {
                            geometries.AddRange(sceneObject.Lods[0].ClipObject.Elements);
                        }
                    }
                }

                glTFConverter.WriteObjectsToGltf(geometries, outputPath);
            });

            EditResourceHeaderCommand = new RelayCommand<EditorScene>(async (EditorScene? sender) =>
            {
                if (sender != null)
                {
                    ResourceHeaderViewModel headerVm = new ResourceHeaderViewModel(sender.Metadata);

                    var modal = new EditResourceHeaderWindow(headerVm);

                    await modal.ShowDialog(MainWindow);
                }
            });

            EditTexturesCommand = new RelayCommand<EditorScene>(async (EditorScene? sender) =>
            {
                if (sender != null)
                {
                    EditTexturesViewModel editTexturesVm = new EditTexturesViewModel(sender.Textures);

                    var modal = new EditTexturesWindow(editTexturesVm);

                    await modal.ShowDialog(MainWindow);
                }
            });

            SaveTexturesCommand = new RelayCommand<EditorScene>(async (EditorScene? sender) =>
            {
                string path = sender.OriginalTextures.Path;

                bool hasPath = !string.IsNullOrEmpty(path);
                string extension = hasPath ? Path.GetExtension(path) : "nxg_textures";

                if (!hasPath || path.StartsWith("dat:"))
                {
                    string outputPath = await MainWindow?.OpenSaveMenu("Save Nxg_Textures", extension);

                    if (outputPath == null) return;

                    sender.OriginalTextures.Path = outputPath;
                }

                var nxg_textures = sender.OriginalTextures;

                int newTextureCount = sender.Textures.Count;

                var rebuiltSet = new NuTextureSet(nxg_textures.TextureSet.Version, nxg_textures.TextureSet.ConversionDate);
                rebuiltSet.Textures = new NuTexture[newTextureCount];
                rebuiltSet.TextureHeaders = new List<NuTexGenHdr>();

                for (int i = 0; i < newTextureCount; i++)
                {
                    rebuiltSet.Textures[i] = sender.Textures[i].Original;
                    rebuiltSet.TextureHeaders.Add(sender.Textures[i].Original.Header);
                }

                sender.OriginalTextures.TextureSet = rebuiltSet;

                using (RawFile file = RawFile.Create(sender.OriginalTextures.Path))
                {
                    SchemaSerializer schema = new SchemaSerializer(file, true);
                    sender.OriginalTextures.Handle(schema, 0);
                }
            });
        }

        public void Initialize()
        {
        }

        private readonly Queue<Action> glQueue = new();

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this,
                new PropertyChangedEventArgs(propertyName));
        }

        public void EnqueueGL(Action action)
        {
            lock (glQueue)
            {
                glQueue.Enqueue(action);
            }
        }

        private void ExecuteGLQueue()
        {
            while (true)
            {
                Action action;

                lock (glQueue)
                {
                    if (glQueue.Count == 0)
                        return;

                    action = glQueue.Dequeue();
                }

                action();
            }
        }

        public void ShowMessageDialog(string title, IEnumerable<string> messages)
        {
            Dispatcher.UIThread.Post(async () =>
            {
                MessageWindow problemModal = new MessageWindow(title, (IEnumerable<string>)messages);

                await problemModal.ShowDialog(MainWindow);
            });
        }

        private void ShowSceneLoadProblems(List<string> problems)
        {
            if (problems == null || problems.Count == 0) return;

            Dispatcher.UIThread.Post(async () =>
            {
                MessageWindow problemModal = new MessageWindow("Problems when opening file!", (IEnumerable<string>)problems);

                await problemModal.ShowDialog(MainWindow);
            });
        }

        public void AddScene(string path)
        {
            string ext = Path.GetExtension(path).ToLower();
            if (ext == ".gsc" || ext == ".ghg")
            {
                Scenes.Add(GSceneConverter.FromGScene(path, out List<string> problems));
                ShowSceneLoadProblems(problems);
            }
        }

        public void AddScene(GScene gscene, NxgTextures nxg_textures, NxgTextures? cubemap_textures)
        {
            Scenes.Add(GSceneConverter.FromGScene(gscene, nxg_textures, cubemap_textures, out List<string> problems));
            ShowSceneLoadProblems(problems);
        }

        public void Render()
        {
            //ExecuteGLQueue();

            //Renderer.Render(Scenes.ToList(), Camera);
        }

        public void OnClick(int x, int y)
        {
            ((ViewportRenderer)Renderer).Pick(x, y, (obj) =>
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    SelectedHierarchyObject = obj;
                });
            });
        }

        public void SetWidthHeight(int width, int height)
        {
            //EnqueueGL(() =>
            //{
            //    GL.Viewport(0, 0, width, height);
            //    //Renderer.SetFramebufferSize(width, height);
            //});

            Camera.SetProjection(width, height);
        }
    }
}
