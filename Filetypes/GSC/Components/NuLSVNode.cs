using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Filetypes.GSC.Components
{
    public class NuLSVNode : IVectorSerializable
    {
        public void Deserialize(RawFile file)
        {
            for (int i = 0; i < 8; i++)
            {
                file.ReadShort(true);
            }
        }
    }
}
