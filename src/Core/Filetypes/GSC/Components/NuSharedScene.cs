using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuSharedScene : ISchemaSerializable
    {
        public string ResourceId;
        public string ObjectId;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandlePascalString(ref ResourceId, 1);
            schema.HandlePascalString(ref ObjectId, 1);
        }
    }
}
