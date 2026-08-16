using Diorama.Core.Filetypes.GSC.Components;
using Diorama.UI.Controls;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.Editor
{
    public class EditorJoint : EditableItem, INamedItem
    {
        public NuJointData Original;

        public string Name { get => Original.Name; set => Set(ref Original.Name, value); }

        public Matrix4 WorldTransform { get; set; }
    }
}
