using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuMtlOldReferencedMaterial : IVectorSerializable, ISchemaSerializable
    {
        public uint ReplacedMaterialIndex;
        public string SourceGsc;
        public string MaterialName;

        public void Deserialize(RawFile file, uint parentVersion)
        {
            uint replacedMaterialIndex = file.ReadUInt(true);
            string sourceGsc = file.ReadPascalString();
            string materialName = file.ReadPascalString();
        }

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandleUInt(ref ReplacedMaterialIndex);
            schema.HandlePascalString(ref SourceGsc, 1);
            schema.HandlePascalString(ref MaterialName, 1);
        }

        public void Serialize(RawFile file, uint parentVersion)
        {
            throw new NotImplementedException();
        }
    }
}
