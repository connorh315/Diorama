using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuOccluderBlock : ISchemaSerializable
    {
        public uint Version;
        public List<NuOccluder> Occluders;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.Expect("BCCO");
            schema.HandleUInt(ref Version);
            if (Version < 2)
            {
                schema.HandleSchemaVarArray(ref Occluders);
            }
            else
            {
                schema.HandleSchemaVector(ref Occluders);
            }
        }
    }
}
