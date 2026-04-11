using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Tests.GameTests
{
    [TestClass]
    public class LmaTests : GscTestBase
    {
        private const string Path = @"G:\SteamLibrary\steamapps\common\LEGO Marvel's Avengers";

        [TestMethod]
        public void DeserializeAllGscFiles()
        {
            DeserializeAll(Path);
        }

        [TestMethod]
        public void ReserializeAllGscFiles()
        {
            ReserializeAll(Path);
        }
    }
}
