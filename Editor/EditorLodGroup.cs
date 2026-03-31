using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Editor
{
    public class EditorLodGroup : IHierarchySelectable
    {
        public string Name => $"LOD Group {Index}";

        public IEnumerable<IHierarchySelectable> Children => ClipObject.Elements;

        public EditorClipObject ClipObject { get; set; }

        public int Index { get; set; }

        public EditorLodGroup(int index)
        {
            Index = index;
        }
    }
}
