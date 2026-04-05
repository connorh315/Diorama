using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuBlendShapeAnim : IVectorSerializable, ISchemaSerializable
    {
        public uint AnimNameIndex;
        public List<NuBlendShapeAnimKey> Keys;

        public void Deserialize(RawFile file, uint parentVersion)
        {
            uint animNameIndex = file.ReadUInt(true);

            List<NuBlendShapeAnimKey> keys = NuSerializer.ReadLegacyVarArray<NuBlendShapeAnimKey>(file, parentVersion);
        }

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandleUInt(ref AnimNameIndex);
            schema.HandleSchemaVarArray(ref Keys, parentVersion);
        }

        public void Serialize(RawFile file, uint parentVersion)
        {
            throw new NotImplementedException();
        }
    }
}
