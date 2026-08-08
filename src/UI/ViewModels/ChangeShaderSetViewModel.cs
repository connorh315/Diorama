using Diorama.Editor;
using Diorama.Editor.ShaderSystem;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;

namespace Diorama.UI.ViewModels
{
    public class ChangeShaderSetViewModel : EditableItem
    {
        public EditorMaterial Original { get; set; }

        private EditorShaderFingerprint selected;
        public EditorShaderFingerprint Selected 
        { 
            get => selected; 
            set
            {
                Set(ref selected, value);

                if (selected == null)
                    return;

                var layout = EditorShaderSystem.Cache.fingerprintLayout;

                for (int i = 0; i < layout.Count; i++)
                {
                    Comparisons[i].UpdateSelected(selected.Fingerprint.Properties[i].ToString());
                    Console.WriteLine($"{layout[i].PropertyName}: {selected.Fingerprint.Properties[i]}");
                }
            }
        }

        public ObservableCollection<EditorShaderFingerprint> Fingerprints { get; }

        private ObservableCollection<EditorShaderFingerprint> filteredInternal = new();
        public ObservableCollection<EditorShaderFingerprint> Filtered { get; set; }

        public ObservableCollection<EditorShaderComparison> Comparisons { get; } = new();

        public void ChangeFingerprint()
        {
            Original.Fingerprint = Selected;
            Original.FingerprintChanged = true;
        }

        private string search = "";
        public string SearchBox
        {
            get => search;
            set
            {
                if (search == value) return;
                Set(ref search, value);

                ApplyFilter();
            }
        }

        public void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                Filtered = Fingerprints;
                OnPropertyChanged(nameof(Filtered));

                return;
            }

            if (Fingerprints == Filtered)
            {
                Filtered = filteredInternal;
                OnPropertyChanged(nameof(Filtered));
            }

            string normalised = search.ToLower();

            Filtered.Clear();

            foreach (var f in Fingerprints)
            {
                if (f.MaterialName.ToLower().Contains(normalised) || f.SceneName.ToLower().Contains(normalised))
                {
                    Filtered.Add(f);
                }
            }
        }

        public ChangeShaderSetViewModel(EditorMaterial original)
        {
            var properties = EditorShaderSystem.GetProperties(original);

            ShaderFingerprint current = new ShaderFingerprint(original, properties);

            int threshold = (int)Math.Floor(0.5f * properties.Length); // at least half the properties have to be the same

            List<EditorShaderFingerprint> suitable = new();

            foreach ((string sceneName, ShaderFingerprint fingerprint) in EditorShaderSystem.Enumerate())
            {
                int score = 0;
                for (int i = 0; i < properties.Length; i++)
                {
                    if (fingerprint.Properties[i].Equals(current.Properties[i]))
                    {
                        score++;
                    }
                }

                if (score > threshold)
                {
                    var editorFingerprint = new EditorShaderFingerprint()
                    {
                        Fingerprint = fingerprint,
                        SceneName = sceneName,
                        Score = score,
                        Suitability = (int)((double)(score * 100) / properties.Length)
                    };

                    suitable.Add(editorFingerprint);
                }
            }

            if (suitable.Count == 0)
                return;

            Fingerprints = new ObservableCollection<EditorShaderFingerprint>(suitable.OrderByDescending(e => e.Score));

            Filtered = Fingerprints;

            Original = original;

            var layout = EditorShaderSystem.Cache.fingerprintLayout;
            for (int i = 0; i < layout.Count; i++)
            {
                Comparisons.Add(new EditorShaderComparison(layout[i].PropertyName, current.Properties[i].ToString()));
            }
        }
    }
}
