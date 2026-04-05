using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuCpuSkinnedblock : ISchemaSerializable
    {
        public uint Version;

        public List<NuCpuSkinLod> CpuSkinLods;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.Expect("SUPC");
            schema.HandleUInt(ref Version);
            
            if (Version < 3)
            {

            }
            else if (Version == 3)
            {

            }
            else
            {
                schema.HandleSerializableVector(ref CpuSkinLods, parentVersion);
            }
        }
    }
}
