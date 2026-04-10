using Diorama.Editor.Metadata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Diorama.UI.ViewModels
{
    public class ResourceHeaderViewModel
    {
        private EditorMetadata metadata;

        public ObservableCollection<EditorResourceReference> References { get; private set; }

        public ICommand RemoveReferenceCommand { get; set; }

        public ResourceHeaderViewModel(EditorMetadata metadata)
        {
            this.metadata = metadata;

            References = metadata.Resources;

            RemoveReferenceCommand = new RelayCommand<EditorResourceReference>((EditorResourceReference sender) =>
            {
                if (sender is null)
                    return;

                References.Remove(sender);
            });
        }
    }
}
