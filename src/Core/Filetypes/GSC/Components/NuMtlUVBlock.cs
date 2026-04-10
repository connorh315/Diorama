using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuMtlUVBlock : ISchemaSerializable
    {
        public int State;
        public int UVSet;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandleInt(ref State);
            schema.HandleInt(ref UVSet);
        }
    }
}
