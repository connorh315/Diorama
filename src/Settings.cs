using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Diorama
{
    public class Settings
    {
#if DEBUG
        public static string DatLocation = @"F:\PS4Games\CUSA01176\data";
#else
        public static string DatLocation = @"";
#endif

        public static bool ShouldWriteROTV = true;

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

        public const string BuildVersion = "v1.1.0";
    }
}
