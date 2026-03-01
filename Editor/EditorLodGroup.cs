using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Editor
{
    public class EditorLodGroup
    {
        public List<EditorGeometryObject> ClipObjects { get; set; }

        public int Index { get; set; }

        public EditorLodGroup(int index)
        {
            Index = index;

            ClipObjects = new();
        }
    }
}
