using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.Editor.ShaderSystem
{
    public class EditorShaderFingerprint : EditableItem
    {
        public ShaderFingerprint Fingerprint;

        public string MaterialName { get => Fingerprint.MaterialName; }
        public string SceneName { get; set; }
        public int Score { get; set; } = 0;
        public int Suitability { get; set; } = 0;
    }
}
