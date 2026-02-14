using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuFaceOnDisplayItem : IVectorSerializable
    {
        public void Deserialize(RawFile file, uint parentVersion)
        {
            // version < 0x1e has "unusedIsFixedUp"
            uint type = file.ReadUInt(true);
            // version < 0x1f uses legacy array
            List<NuFaceOnInstance> instances = NuSerializer.ReadVectorArray<NuFaceOnInstance>(file);
        }
    }
}
