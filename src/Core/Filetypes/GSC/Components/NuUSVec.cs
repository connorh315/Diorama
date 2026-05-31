using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuUSVec : IVectorSerializable, ISchemaSerializable
    {
        public ushort X;
        public ushort Y;
        public ushort Z;

        public void Deserialize(RawFile file, uint parentVersion)
        {
            ushort x = file.ReadUShort(true);
            ushort y = file.ReadUShort(true);
            ushort z = file.ReadUShort(true);
        }

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandleUShort(ref X);
            schema.HandleUShort(ref Y);
            schema.HandleUShort(ref Z);
        }

        public void Serialize(RawFile file, uint parentVersion)
        {
            throw new NotImplementedException();
        }
    }
}
