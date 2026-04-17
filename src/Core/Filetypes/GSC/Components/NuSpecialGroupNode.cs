using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuSpecialGroupNode : IVectorSerializable, ISchemaSerializable
    {
        public List<short> SpecialIndexes;

        public void Deserialize(RawFile file, uint parentVersion)
        {
            if (parentVersion < 0x21)
            {
                SpecialIndexes = NuSerializer.ReadLegacyVarArray<short>(file);
            }
            else
            {
                SpecialIndexes = NuSerializer.ReadVectorArray<short>(file);
            }
        }

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            if (parentVersion < 0x21)
            {
                schema.HandleLegacyVarArray(ref SpecialIndexes);
            }
            else
            {
                schema.HandleSerializableVector(ref SpecialIndexes);
            }
        }

        public void Serialize(RawFile file, uint parentVersion)
        {
            if (parentVersion < 0x21)
            {
                NuSerializer.WriteLegacyVarArray(file, SpecialIndexes);
            }
            else
            {
                NuSerializer.WriteVectorArray(file, SpecialIndexes);
            }
        }
    }
}
