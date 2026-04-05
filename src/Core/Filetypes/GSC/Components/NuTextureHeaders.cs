using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuTextureHeaders : ISchemaSerializable
    {
        public uint Version;

        public List<NuTextureHeader> Headers;

        public static NuTextureHeaders Read(RawFile file)
        {
            NuTextureHeaders headers = new NuTextureHeaders();
            
            Debug.Assert(file.ReadString(4) == "HGXT");
            Debug.Assert(file.ReadUInt(true) == 0xc);

            headers.Headers = NuSerializer.ReadVectorArray<NuTextureHeader>(file);

            return headers;
        }

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.Expect("HGXT");
            schema.HandleUInt(ref Version);

            schema.HandleSchemaVector(ref Headers);
        }
    }
}
