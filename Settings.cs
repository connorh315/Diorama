using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
