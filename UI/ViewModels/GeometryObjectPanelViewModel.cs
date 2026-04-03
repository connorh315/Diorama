using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Diorama.Editor;
using Diorama.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Diorama.UI.ViewModels
{
    public class GeometryObjectPanelViewModel : INotifyPropertyChanged
    {
        public SceneController Controller { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public GeometryObjectPanelViewModel(SceneController controller)
        {
            Controller = controller;
        }

        public void ReplaceMesh(string path)
        {
            if (string.IsNullOrEmpty(path) || Controller.SelectedGeometry == null) return;

            var selectedGeo = Controller.SelectedGeometry;

            Controller.EnqueueGL(() =>
            {
                RenderMesh newMesh = OBJConverter.MeshFromOBJ(path, selectedGeo.Mesh);

                selectedGeo.Mesh = newMesh;
            });
        }        
    }
}
