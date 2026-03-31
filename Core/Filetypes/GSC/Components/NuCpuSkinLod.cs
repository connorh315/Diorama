using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuCpuSkinLod : IVectorSerializable
    {
        public void Deserialize(RawFile file, uint parentVersion)
        {
            if (parentVersion < 4)
            {

            }
            else
            {
                List<NuCpuSkinLayer> layers = NuSerializer.ReadVectorArray<NuCpuSkinLayer>(file, parentVersion);
                List<short> layerIdTable = NuSerializer.ReadVectorArray<short>(file);
            }
        }

        public void Serialize(RawFile file, uint parentVersion)
        {
            throw new NotImplementedException();
        }
    }
}
