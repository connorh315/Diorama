using Avalonia.Input;
using Avalonia.Threading;
using Diorama.Core.Filetypes.GSC;
using Diorama.Core.Filetypes.GSC.Components;
using Diorama.Core.Filetypes.TEXTURES;
using Diorama.Editor;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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

        public SceneController(IDioramaRenderer renderer)
        {
            Renderer = renderer;
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
            if (Path.GetExtension(path).ToLower() == ".gsc")
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
