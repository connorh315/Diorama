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

        public string Names { get; set; }

        public static NuNameTable Read(RawFile file)
        {
            NuNameTable table = new NuNameTable();

            Debug.Assert(file.ReadString(4) == "LBTN");

            table.Version = file.ReadUInt(true);
            Debug.Assert(table.Version == 0x4f || table.Version == 0x50 || table.Version == 0x52 || table.Version == 0x53 || table.Version == 0x57);

            int ntblLength = file.ReadInt(true);
            table.Names = file.ReadString(ntblLength);

            return table;
        }

        public void Write(RawFile file)
        {
            file.WriteString("LBTN");
            file.WriteUInt(Version, true);
            file.WriteInt(Names.Length + 1, true);
            file.WriteString(Names, 1);
        }
    }
}
