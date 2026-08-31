using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuBlendRunV1 : ISchemaSerializable
    {
        public short First;
        public short Length;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandleShort(ref First);
            schema.HandleShort(ref Length);
        }
    }
}
