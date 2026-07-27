using Diorama.Editor;
using Diorama.Editor.ShaderSystem;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Diorama.UI.ViewModels
{
    public class ChangeShaderSetViewModel : EditableItem
    {
        public EditorMaterial Original { get; set; }

        public EditorShaderFingerprint Selected { get; set; }

        public ObservableCollection<EditorShaderFingerprint> Fingerprints { get; }

        public void ChangeFingerprint()
        {
            Original.Fingerprint = Selected;
            Original.FingerprintChanged = true;
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

            Fingerprints = new ObservableCollection<EditorShaderFingerprint>(suitable.OrderByDescending(e => e.Score));

            Original = original;
        }
    }
}
