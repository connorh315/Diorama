using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuDynamicString : IVectorSerializable, ISchemaSerializable
    {
        public string Value;

        public void Deserialize(RawFile file, uint parentVersion)
        {
            Value = file.ReadPascalString();
        }

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandlePascalString(ref Value, 1);
        }

        public void Serialize(RawFile file, uint parentVersion)
        {
            file.WritePascalString(Value, 1);
        }
    }
}
