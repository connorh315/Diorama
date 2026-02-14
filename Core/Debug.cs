using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core
{
    public class Debug
    {
        public static void Assert(bool condition, bool focus=false)
        {
            if (!condition)
            {
                if (focus)
                {
                    throw new Exception("Yield");
                }
                throw new Exception("");
            }
        }

        public static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                throw new Exception(message);
            }
        }
    }
}
