using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Editor
{
    public class EditorLodGroup : IHierarchySelectable, INotifyPropertyChanged
    {
        public string Name => $"LOD Group {Index}";

        public IEnumerable<IHierarchySelectable> Children => ClipObject.Elements;

        public EditorClipObject ClipObject { get; set; }

        public int Index { get; set; }

        public float FadeDistance { get; set; }


        private bool _isActive;

        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public EditorLodGroup(int index)
        {
            Index = index;
        }
    }
}
