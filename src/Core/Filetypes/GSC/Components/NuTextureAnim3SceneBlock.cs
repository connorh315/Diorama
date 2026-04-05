using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuTextureAnim3SceneBlock : ISchemaSerializable
    {
        public uint Version;
        public List<NuTexAnim3Header> TexAnim3Headers;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.Expect("BNAT");
            schema.HandleUInt(ref Version);
            Debug.Assert(Version == 5);

            schema.HandleSchemaVarArray(ref TexAnim3Headers);
        }
    }
}
