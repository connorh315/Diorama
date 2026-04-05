using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Tests
{
    public class GscFileFailureInfo
    {
        public int Count;
        public List<string> Paths = new();

        public void AddFile(string path)
        {
            if (Count++ < 20)
            {
                Paths.Add(path);
            }
        }
    }
}
