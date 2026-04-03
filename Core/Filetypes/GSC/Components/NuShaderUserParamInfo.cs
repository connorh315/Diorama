using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuShaderUserParamInfo : ISchemaSerializable
    {
        public string Name;
        public uint Type;
        public int Value;
        public short PackedIndex;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandlePascalString(ref Name, 1);
            schema.HandleUInt(ref Type);
            schema.HandleInt(ref Value);
            schema.HandleShort(ref PackedIndex);
        }
    }
}
