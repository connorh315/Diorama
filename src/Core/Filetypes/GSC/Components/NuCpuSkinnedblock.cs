using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuCpuSkinnedblock : ISchemaSerializable
    {
        public uint BlockVersion;
        public uint Version;

        public List<NuCpuSkinLod> CpuSkinLods;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.Expect("SUPC");
            schema.HandleUInt(ref BlockVersion);
            
            if (BlockVersion < 3)
            {
                schema.HandleUInt(ref Version);
                schema.HandleSchemaVarArray(ref CpuSkinLods, Version);
            }
            else if (BlockVersion == 3)
            {
                schema.HandleSchemaVarArray(ref CpuSkinLods, parentVersion);
            }
            else
            {
                schema.HandleSchemaVector(ref CpuSkinLods, parentVersion);
            }
        }
    }
}
