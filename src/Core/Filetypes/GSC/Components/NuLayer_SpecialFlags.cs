using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuLayer_SpecialFlags : IVectorSerializable
    {
        public void Deserialize(RawFile file, uint parentVersion)
        {
            byte type = file.ReadByte();
            byte jointIndex = file.ReadByte();
            short specialIndex = file.ReadShort(true);
            if (parentVersion > 7)
            {
                byte layer = file.ReadByte();
            }
        }

        public void Serialize(RawFile file, uint parentVersion)
        {
            throw new NotImplementedException();
        }
    }
}
