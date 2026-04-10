using Diorama.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Diorama.Editor.Metadata
{
    public class EditorMetadata
    {
        public ObservableCollection<EditorResourceReference> Resources { get; } = new();

        
    }
}
