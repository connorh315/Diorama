using BrickVault;
using BrickVault.Types;
using Diorama.Editor;
using Diorama.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using static System.Net.WebRequestMethods;

namespace Diorama.UI.ViewModels
{
    public class OpenFromArchiveViewModel : EditableItem
    {
        public ObservableCollection<FileLocation> Filtered { get; } = new();

        public FileLocation Selected { get; set; }

        public bool Commited = false;

        private string search = "";
        public string Search
        {
            get => search;
            set
            {
                if (search == value) return;

                search = value;

                ApplyFilter();
            }
        }

        private List<FileLocation> paths;

        public void ApplyFilter()
        {
            Filtered.Clear();

            string sanitisedSearch = Search.ToLower();

            foreach (var location in paths)
            {
                if (location.FullPath.Contains(sanitisedSearch, StringComparison.OrdinalIgnoreCase))
                {
                    Filtered.Add(location);
                }
            }
        }

        public bool OpenSelected()
        {
            if (Selected != null)
            {
                Commited = true;
                return true;
            }

            return false;
        }

        public OpenFromArchiveViewModel(List<FileLocation> filePaths)
        {
            this.paths = filePaths;

            ApplyFilter();
        }
    }
}
