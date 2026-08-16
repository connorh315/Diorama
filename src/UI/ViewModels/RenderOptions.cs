using Diorama.Editor;
using Diorama.Editor.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.UI.ViewModels
{
    public class RenderOptions : EditableItem
    {
        [DisplayLabel("Only Color 0")]
        public static bool Color0 { get; set; } = false;

        [DisplayLabel("Only Color 0 R")]
        public static bool Color0R { get; set; } = false;

        [DisplayLabel("Only Color 0 G")]
        public static bool Color0G { get; set; } = false;

        [DisplayLabel("Only Color 0 B")]
        public static bool Color0B { get; set; } = false;

        [DisplayLabel("Only Color 0 A")]
        public static bool Color0A { get; set; } = false;

        [DisplayLabel("Only Color 1")]
        public static bool Color1 { get; set; } = false;

        [DisplayLabel("Only Color 1 R")]
        public static bool Color1R { get; set; } = false;

        [DisplayLabel("Only Color 1 G")]
        public static bool Color1G { get; set; } = false;

        [DisplayLabel("Only Color 1 B")]
        public static bool Color1B { get; set; } = false;

        [DisplayLabel("Only Color 1 A")]
        public static bool Color1A { get; set; } = false;

        [DisplayLabel("Show Specular")]
        public static bool ShowSpecular { get; set; } = true;

        [DisplayLabel("Show Only Env Map")]
        public static bool ShowOnlyEnvMap { get; set; } = false;

        public static bool ShowPoIs { get; set; } = false;
    }
}
