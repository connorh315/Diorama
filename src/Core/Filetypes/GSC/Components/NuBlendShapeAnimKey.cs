using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuBlendShapeAnimKey : IVectorSerializable, ISchemaSerializable
    {
        public short Index;
        public short IndexTo;
        public byte Blend;
        public byte FlatlineRegion;

        public void Deserialize(RawFile file, uint parentVersion)
        {
            short index = file.ReadShort(true);
            short indexTo = file.ReadShort(true);
            byte blend = file.ReadByte();
            if (parentVersion > 3)
            {
                byte flatlineRegion = file.ReadByte();
            }
            // version > 3
        }

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandleShort(ref Index);
            schema.HandleShort(ref IndexTo);
            schema.HandleByte(ref Blend);
            if (parentVersion > 3)
            {
                schema.HandleByte(ref FlatlineRegion);
            }
        }

        public void Serialize(RawFile file, uint parentVersion)
        {
            throw new NotImplementedException();
        }
    }
}
