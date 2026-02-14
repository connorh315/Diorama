using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuSpecialGroupNode : IVectorSerializable
    {
        public void Deserialize(RawFile file, uint parentVersion)
        {
            if (parentVersion < 0x21)
            {
                List<short> specialIndexes = NuSerializer.ReadLegacyVarArray<short>(file);
            }
            else
            {
                List<short> specialIndexes = NuSerializer.ReadVectorArray<short>(file);
            }
        }
    }
}
