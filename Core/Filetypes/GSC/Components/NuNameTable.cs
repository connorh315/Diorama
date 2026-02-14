using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuNameTable
    {
        public uint Version;

        public static NuNameTable Read(RawFile file)
        {
            NuNameTable table = new NuNameTable();

            Debug.Assert(file.ReadString(4) == "LBTN");

            table.Version = file.ReadUInt(true);
            Debug.Assert(table.Version == 0x4f || table.Version == 0x50 || table.Version == 0x53);

            int ntblLength = file.ReadInt(true);
            string ntblData = file.ReadString(ntblLength);

            return table;
        }
    }
}
