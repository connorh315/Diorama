using Avalonia.Input;
using Avalonia.Threading;
using Diorama.Core;
using Diorama.Core.Filetypes.GSC;
using Diorama.Core.Filetypes.GSC.Components;
using Diorama.Core.Filetypes.TEXTURES;
using Diorama.Editor;
using Diorama.UI;
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
        public ObservableCollection<EditorScene> Scenes { get; } = new();
        public IDioramaRenderer Renderer;

        public bool IsInitialized = false;

        public EditorSceneObject? SelectedSceneObject =>
            SelectedHierarchyObject switch
            {
                EditorSceneObject obj => obj,
                EditorGeometryObject geo => geo.Parent.Parent,
                _ => null
            };

        public EditorGeometryObject? SelectedGeometry =>
            SelectedHierarchyObject as EditorGeometryObject;

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
            }
        }

        public ICommand SaveSceneCommand { get; }

        public ICommand RemoveSceneCommand { get; }

        public SceneController(IDioramaRenderer renderer)
        {
            Renderer = renderer;

            SaveSceneCommand = new RelayCommand<EditorScene>((EditorScene? sender) =>
            {
                string path = sender.OriginalScene.Path.Replace(".GSC", "_1.GSC");

                using (RawFile file = new RawFile(path))
                {
                    GSerializationContext ctx = new GSerializationContext();
                    sender.OriginalScene.Write(file, ctx);
                }
            });

            RemoveSceneCommand = new RelayCommand<EditorScene>((EditorScene? sender) =>
            {
                if (sender != null)
                {
                    Scenes.Remove(sender);
                }
            });
        }

        public void Initialize()
        {
            IsInitialized = true;

            Renderer.Initialize();
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

        public void AddScene(string path)
        {
            string ext = Path.GetExtension(path).ToLower();
            if (ext == ".gsc" || ext == ".ghg")
            {
                Scenes.Add(GSceneConverter.FromGScene(path));
            }
        }

        public void Render()
        {
            ExecuteGLQueue();

            Renderer.Render(Scenes.ToList());
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
    }
}
