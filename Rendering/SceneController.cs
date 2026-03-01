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

        private EditorSceneObject? selected;
        public EditorSceneObject? Selected 
        { 
            get => selected; 
            set
            {
                if (selected == value)
                    return;

                selected = value;
                OnPropertyChanged();
            }
        }

        private EditorGeometryObject? selectedGeometry;
        public EditorGeometryObject? SelectedGeometry
        {
            get => selectedGeometry;
            set
            {
                if (selectedGeometry == value)
                    return;
                selectedGeometry = value;
                OnPropertyChanged();
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

        private readonly Queue<string> pendingSceneLoads = new();

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this,
                new PropertyChangedEventArgs(propertyName));
        }

        public void LoadScene(string filePath)
        {
            lock (pendingSceneLoads)
            {
                pendingSceneLoads.Enqueue(filePath);
            }
        }

        private void LoadPendingScenes()
        {
            while (true)
            {
                string path;

                lock (pendingSceneLoads)
                {
                    if (pendingSceneLoads.Count == 0)
                        return;

                    path = pendingSceneLoads.Dequeue();
                }

                if (Path.GetExtension(path).ToLower() == ".gsc")
                {
                    Scenes.Add(GSceneConverter.FromGScene(path));
                }
            }

        }

        public void Render()
        {
            LoadPendingScenes();

            Renderer.Render(Scenes.ToList());
        }

        public void OnClick(int x, int y)
        {
            ((ViewportRenderer)Renderer).Pick(x, y, (obj) =>
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    SelectedGeometry = obj;
                    Selected = obj?.Parent;
                });
            });
        }
    }
}
