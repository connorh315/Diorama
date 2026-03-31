using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Editor
{
    public interface IHierarchySelectable
    {
        public string Name { get; }

        public IEnumerable<IHierarchySelectable> Children { get; }
    }
}
