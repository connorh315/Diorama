using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Filetypes.GSC.Components
{
    public class NuTransformMtx : IVectorSerializable
    {
        public void Deserialize(RawFile file)
        {
            for (int i = 0; i < 12; i++)
                file.ReadFloat(true);
        }
    }
}
