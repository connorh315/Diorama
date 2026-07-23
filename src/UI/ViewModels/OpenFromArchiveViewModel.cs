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
        public ObservableCollection<ArchiveFile> Filtered { get; } = new();

        public ArchiveFile Selected { get; set; }

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

        private Dictionary<DATFile, List<ArchiveFile>> archives;

        public void ApplyFilter()
        {
            Filtered.Clear();

            string sanitisedSearch = Search.ToLower();

            foreach (var archive in archives.Values)
            {
                foreach (var file in archive)
                {
                    if (file.Path.ToLower().Contains(sanitisedSearch))
                    {
                        Filtered.Add(file);
                    }
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

        public OpenFromArchiveViewModel(Dictionary<DATFile, List<ArchiveFile>> archives)
        {
            this.archives = archives;

            ApplyFilter();
        }
    }
}
