using Diorama.UI;
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ComponentModel.DataAnnotations;

namespace Diorama.Editor.Metadata
{
    public class EditorResourceReference : INotifyPropertyChanged
    {
        public uint ConvertPathToType(string path)
        {
            string extension = Path.GetExtension(path).ToLower();
            switch (extension)
            {
                case ".tex":
                    return 5;
                case ".nxg_textures":
                    return 13;
                case ".durango_shaders":
                case ".ps4_shaders":
                case ".pc_shaders":
                case ".mac_shaders":
                case ".shaders":
                    return 14;
                case ".gsc":
                    return 1;
            }
            return uint.MaxValue;
        }

        public uint ConvertPathToClass(string path)
        {
            string extension = Path.GetExtension(path).ToLower();
            switch (extension)
            {
                case ".tex":
                    return 0x4fffffff;
                case ".nxg_textures":
                    return 0xf00000bd;
                case ".durango_shaders":
                    return 0xf0000004;
                case ".ps4_shaders":
                    return 0xf0000010;
                case ".pc_shaders":
                    return 0xf0000020;
                case ".mac_shaders":
                    return 0xf0000008;
                case ".shaders":
                case ".gsc":
                    return 0xffffffff;
                default:
                    return 0xffffffff;
            }
        }

        private string path;

        [FileExtensions(ErrorMessage = "Filetype not supported in Resource Header", Extensions = "tex,nxg_textures,durango_shaders,ps4_shaders,pc_shaders,mac_shaders,shaders,gsc")]
        public string FilePath
        {
            get => path;
            set
            {
                if (path == value) return;

                path = value;

                uint type = ConvertPathToType(path);
                uint platformClass = ConvertPathToClass(path);
                if (type != uint.MaxValue)
                {
                    Type = type;
                    PlatformsAndClasses = platformClass;
                }

                OnPropertyChanged(nameof(FilePath));
                OnPropertyChanged(nameof(Type));
                OnPropertyChanged(nameof(PlatformsAndClasses));
                OnPropertyChanged(nameof(PlatformsAndClassesHex));
            }
        }

        public uint Type { get; set; }

        public uint PlatformsAndClasses { get; set; }
        public string PlatformsAndClassesHex
        {
            get => PlatformsAndClasses.ToString("X");
            set
            {
                if (uint.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out var result))
                    PlatformsAndClasses = result;
            }
        }

        public byte[] Checksum;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
