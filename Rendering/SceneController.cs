using Avalonia.Input;
using Avalonia.Threading;
using Diorama.Core.Filetypes.GSC;
using Diorama.Core.Filetypes.GSC.Components;
using Diorama.Core.Filetypes.TEXTURES;
using Diorama.Editor;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Rendering
{
    public class SceneController
    {
        public List<EditorScene> Scenes = new();
        public IDioramaRenderer Renderer;

        public bool IsInitialized = false;

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

            Renderer.Render(Scenes);
        }
    }
}
