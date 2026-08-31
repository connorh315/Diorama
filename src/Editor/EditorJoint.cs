using Avalonia.Threading;
using Diorama.Core.Filetypes.GSC.Components;
using Diorama.Editor.Attributes;
using Diorama.UI.Controls;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Diorama.Editor
{
    public class EditorJoint : EditableItem, INamedItem, IHierarchySelectable
    {
        public NuJointData Original;

        private EditorJoint parent;
        public EditorJoint Parent
        {
            get => parent;
            set
            {
                if (value == null)
                    return;

                var oldParent = parent;
                parent = value;

                parent?.Children.Add(this);
                Dispatcher.UIThread.Post(() =>
                {
                    oldParent?.Children.Remove(this);
                }, DispatcherPriority.Default);

                OnPropertyChanged(nameof(Parent));
            }
        }

        public EditorScene ParentScene { get; }

        public EditorJoint(NuJointData original, Matrix4 transform, EditorScene parentScene)
        {
            Original = original;
            WorldTransform = transform;
            ParentScene = parentScene;
        }

        public ObservableCollection<EditorJoint> Children { get; } = new();

        [DisplayLabel("Joint Name")]
        public string Name { get => Original.Name; set { Set(ref Original.Name, value); OnPropertyChanged(nameof(DisplayName)); } }

        public string DisplayName { get => Original.Name; }

        [DisplayLabel("Joint Transform")]
        public Matrix4 WorldTransform { get; set; }

        public List<bool> JointOnLod = new();

        IEnumerable<IHierarchySelectable> IHierarchySelectable.Children => Children.Cast<IHierarchySelectable>();
    }
}
