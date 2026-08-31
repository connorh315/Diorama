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

        public IEnumerable<IHierarchySelectable> Children
        {
            get
            {
                if (ClipObject != null)
                {
                    foreach (var el in ClipObject?.Elements)
                    {
                        yield return el;
                    }
                }

                if (Spare != null)
                {
                    foreach (var clip in Spare)
                    {
                        foreach (var el in clip?.Elements)
                        {
                            yield return el;
                        }
                    }
                }
            }
        }

        public EditorClipObject ClipObject { get; set; }

        public List<EditorClipObject> Spare { get; set; }

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
