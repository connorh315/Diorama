using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Filetypes.GSC.Components
{
    public class NuLayerData : IVectorSerializable
    {
        public void Deserialize(RawFile file, uint parentVersion)
        {
            // lots of version rubbish in this

            string name = file.ReadPascalString();
            short metaDataIndex = file.ReadShort(true);
            short numRigids = file.ReadShort(true);
            short numSkins = file.ReadShort(true);
        }
    }
}
