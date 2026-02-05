using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Filetypes.GSC.Components
{
    public class NuVec4 : IVectorSerializable
    {
        public void Deserialize(RawFile file)
        {
            file.ReadFloat(true);
            file.ReadFloat(true);
            file.ReadFloat(true);
            file.ReadFloat(true);
        }
    }
}
