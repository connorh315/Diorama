using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuBlendShapeAnimList : IVectorSerializable, ISchemaSerializable
    {
        public int SpecialObjectIndex;

        public List<NuBlendShapeAnim> Anims;

        public List<uint> Locators;
        public List<NuMtx> LocatorPos;

        public void Deserialize(RawFile file, uint parentVersion)
        {
            int specialObjectIndex = file.ReadInt(true);

            List<NuBlendShapeAnim> anims = NuSerializer.ReadLegacyVarArray<NuBlendShapeAnim>(file, parentVersion);

            // if (version > 3) NEEDS SOME WORK!
            if (parentVersion > 2)
            {
                List<uint> locators = NuSerializer.ReadLegacyVarArray<uint>(file);
                List<NuMtx> locatorPos = NuSerializer.ReadLegacyVarArray<NuMtx>(file);
            }
        }

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandleInt(ref SpecialObjectIndex);

            schema.HandleSchemaVarArray(ref Anims, parentVersion);

            if (parentVersion > 2)
            {
                schema.HandleSchemaVarArray(ref Locators);
                schema.HandleSchemaVarArray(ref LocatorPos);
            }
        }

        public void Serialize(RawFile file, uint parentVersion)
        {
            throw new NotImplementedException();
        }
    }
}
