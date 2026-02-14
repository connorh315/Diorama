using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuBlendShapeAnimList : IVectorSerializable
    {
        public void Deserialize(RawFile file, uint parentVersion)
        {
            int specialObjectIndex = file.ReadInt(true);

            List<NuBlendShapeAnim> anims = NuSerializer.ReadLegacyVarArray<NuBlendShapeAnim>(file, parentVersion);

            // if (version > 3) NEEDS SOME WORK!
            if (parentVersion > 2)
            {
                List<uint> locators = NuSerializer.ReadLegacyVarArray<uint>(file);
                List<NuMtx> locatorPos = NuSerializer.ReadLegacyVarArray<NuMtx>(file);
            }
        }
    }
}
