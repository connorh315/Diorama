using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Filetypes.GSC.Components
{
    public class NuBlendShapeAnimKey : IVectorSerializable
    {
        public void Deserialize(RawFile file, uint parentVersion)
        {
            short index = file.ReadShort(true);
            short indexTo = file.ReadShort(true);
            byte blend = file.ReadByte();
            if (parentVersion > 3)
            {
                byte flatlineRegion = file.ReadByte();
            }
            // version > 3
        }
    }
}
