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

            var scene = Controller.SelectedGeometry.Parent.SceneOwner;

            Controller.EnqueueGL(() =>
            {
                RenderMesh newMesh = OBJConverter.MeshFromOBJ(path, selectedGeo.Mesh, scene);

                selectedGeo.Mesh = newMesh;
            });
        }

        public void ExportMesh(string path)
        {
            if (Controller.SelectedGeometry == null) return;

            var selectedGeo = Controller.SelectedGeometry;

            OBJConverter.WriteMeshToOBJ(selectedGeo.Mesh, path);

        }
    }
}
