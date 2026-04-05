using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuMtlUVBlock : ISchemaSerializable
    {
        public uint State;
        public uint UVSet;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandleUInt(ref State);
            schema.HandleUInt(ref UVSet);
        }
    }
}
