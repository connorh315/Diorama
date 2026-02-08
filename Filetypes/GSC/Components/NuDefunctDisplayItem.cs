using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Filetypes.GSC.Components
{
    public class NuDefunctDisplayItem : IVectorSerializable
    {
        public void Deserialize(RawFile file, uint parentVersion)
        {
            byte type = file.ReadByte();
            byte id = file.ReadByte();
            uint index = file.ReadUInt(true);
        }
    }
}
