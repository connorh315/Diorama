using Diorama.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Filetypes.GSC.Components
{
    public class NuCpuSkinBones : IVectorSerializable
    {
        public void Deserialize(RawFile file, uint parentVersion)
        {
            if (parentVersion < 4)
            {

            }
            else
            {
                List<ushort> vertIndexs = NuSerializer.ReadVectorArray<ushort>(file);
                List<byte> vertWeights = NuSerializer.ReadVectorArray<byte>(file);
            }
        }
    }
}
