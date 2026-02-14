using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuVec : IVectorSerializable
    {
        public void Deserialize(RawFile file, uint parentVersion)
        {
            file.ReadFloat(true);
            file.ReadFloat(true);
            file.ReadFloat(true);
        }
    }
}
