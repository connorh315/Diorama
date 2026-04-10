using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components.RESH
{
    public class NuCheckSum : ISchemaSerializable
    {
        public byte[] Checksum;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandleArray(ref Checksum, 16);
        }
    }
}
