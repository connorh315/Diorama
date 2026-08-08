using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.Tests.GameTests
{
    [TestClass]
    public class LdcsvTests : GscTestBase
    {
        private const string Path = @"G:\SteamLibrary\steamapps\common\LEGO DC Super-Villains";

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
