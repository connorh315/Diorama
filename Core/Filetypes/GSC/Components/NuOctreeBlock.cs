using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuOctreeBlock : ISchemaSerializable
    {
        public uint Version;
        public List<NuLSVOctree> LSVOctree;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.Expect("5LVI");
            schema.HandleUInt(ref Version);

            if (Version >= 3)
            {
                schema.HandleSchemaVector(ref LSVOctree);
            }
            else
            {
                schema.HandleSchemaVarArray(ref LSVOctree);
            }
        }
    }
}
