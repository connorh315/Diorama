using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuLSVNode : ISchemaSerializable
    {
        public short[] Values = new short[8];

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            for (int i = 0; i < 8; i++)
            {
                schema.HandleShort(ref Values[i]);
            }
        }
    }
}
