using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Filetypes.GSC.Components
{
    public class NuNameTable
    {
        public static NuNameTable Read(RawFile file)
        {
            NuNameTable table = new NuNameTable();

            Debug.Assert(file.ReadString(4) == "LBTN");

            uint ntblVersion = file.ReadUInt(true);
            Debug.Assert(ntblVersion == 0x4f || ntblVersion == 0x50);

            int ntblLength = file.ReadInt(true);
            string ntblData = file.ReadString(ntblLength);

            return table;
        }
    }
}
