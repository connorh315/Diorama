using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuUSVec : IVectorSerializable
    {
        public void Deserialize(RawFile file, uint parentVersion)
        {
            ushort x = file.ReadUShort(true);
            ushort y = file.ReadUShort(true);
            ushort z = file.ReadUShort(true);
        }

        public void Serialize(RawFile file, uint parentVersion)
        {
            throw new NotImplementedException();
        }
    }
}
