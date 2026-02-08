using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Filetypes.GSC.Components
{
    public class NuMtlOldReferencedMaterial : IVectorSerializable
    {
        public void Deserialize(RawFile file, uint parentVersion)
        {
            uint replacedMaterialIndex = file.ReadUInt(true);
            string sourceGsc = file.ReadPascalString();
            string materialName = file.ReadPascalString();
        }
    }
}
