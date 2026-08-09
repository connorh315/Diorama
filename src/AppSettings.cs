using Diorama.Editor;
using Diorama.Editor.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Diorama
{
    public class AppSettings : EditableItem
    {
        public const string AppName = "Diorama";
        public const string BuildVersion = "v1.2.0";

        public static string AppString => $"{AppName} {BuildVersion}";

        public static AppSettings Settings = Load();

        private const string settingsFile = "settings.txt";

        private string datLocation;

        [DisplayLabel("DAT Archives Location")]
        public string DatLocation { get => datLocation; set => Set(ref datLocation, value); }

        public static bool ShouldWriteROTV = false;

        public static string BuildDate => Assembly
            .GetExecutingAssembly()
            .GetCustomAttributes<AssemblyMetadataAttribute>()
            .FirstOrDefault(a => a.Key == "BuildDate")
            ?.Value;

        public static string BuildType => Assembly
            .GetExecutingAssembly()
            .GetCustomAttributes<AssemblyMetadataAttribute>()
            .FirstOrDefault(a => a.Key == "PublishType")
            ?.Value;

        public void Save()
        {
            if (DoNotSave) return;

            var lines = new List<string>();

            foreach (var prop in typeof(AppSettings).GetProperties())
            {
                if (prop.CanRead)
                {
                    var value = prop.GetValue(this)?.ToString() ?? string.Empty;
                    lines.Add($"{prop.Name}={value}");
                }
            }

            File.WriteAllLines(settingsFile, lines);
        }

        private static bool DoNotSave = false;

        public static AppSettings Load()
        {
            DoNotSave = true;

            var settings = new AppSettings();

            if (!File.Exists(settingsFile))
                return settings;

            bool inSection = false;
            string section = string.Empty;

            foreach (var line in File.ReadAllLines(settingsFile))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    inSection = true;
                    section = line[1..^1];
                    continue;
                }

                if (!inSection)
                {
                    var parts = line.Split('=', 2);
                    if (parts.Length != 2) continue;

                    var prop = typeof(AppSettings).GetProperty(parts[0]);
                    if (prop?.CanWrite == true)
                    {
                        try
                        {
                            var value = TypeDescriptor
                                .GetConverter(prop.PropertyType)
                                .ConvertFromString(parts[1]);
                            prop.SetValue(settings, value);
                        }
                        catch { }
                    }
                }
            }

            DoNotSave = false;

            return settings;
        }

    }
}
