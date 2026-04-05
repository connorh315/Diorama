using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Tests
{
    [TestClass]
    public class DimensionsTests : GscTestBase
    {
        private const string Path = @"F:\PS4Games\CUSA01176";

        [TestMethod]
        public void DeserializeAllGscFiles()
        {
            DeserializeAll(Path);
        }
    }
}
