using Diorama.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Filetypes.GSC.Components
{
    public class NuBlendShapeAnim : IVectorSerializable
    {
        public void Deserialize(RawFile file, uint parentVersion)
        {
            uint animNameIndex = file.ReadUInt(true);

            List<NuBlendShapeAnimKey> keys = NuSerializer.ReadLegacyVarArray<NuBlendShapeAnimKey>(file, parentVersion);
        }
    }
}
