using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.UI.Controls
{
    public interface INamedItem
    {
        public string Name { get; set; }

        public string DisplayName { get; }
    }
}
