using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuLayerData : IVectorSerializable, ISchemaSerializable
    {
        public string Name;
        public short MetaDataIndex;
        public short NumRigids;
        public short NumSkins;

        public void Deserialize(RawFile file, uint parentVersion)
        {
            // lots of version rubbish in this

            string name = file.ReadPascalString();
            short metaDataIndex = file.ReadShort(true);
            short numRigids = file.ReadShort(true);
            short numSkins = file.ReadShort(true);
        }

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandlePascalString(ref Name);
            schema.HandleShort(ref MetaDataIndex);
            schema.HandleShort(ref NumRigids);
            schema.HandleShort(ref NumSkins);
        }

        public void Serialize(RawFile file, uint parentVersion)
        {
            throw new NotImplementedException();
        }
    }
}
