using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Filetypes.GSC.Components
{
    public class NuDynamicString : IVectorSerializable
    {
        public string Value;

        public void Deserialize(RawFile file, uint parentVersion)
        {
            Value = file.ReadPascalString();
        }
    }
}
