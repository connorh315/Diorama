using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuShaderUserTextureInfo : ISchemaSerializable
    {
        public string Name;
        public uint Type;
        public string Value;
        public short PackedIndex;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandlePascalString(ref Name, 1);
            schema.HandleUInt(ref Type);
            schema.HandlePascalString(ref Value, 1);
            schema.HandleShort(ref PackedIndex);
        }
    }
}
