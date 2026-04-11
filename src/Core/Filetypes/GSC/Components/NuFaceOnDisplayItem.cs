using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuFaceOnDisplayItem : IVectorSerializable, ISchemaSerializable
    {
        public uint Type;
        public List<NuFaceOnInstance> Instances;

        public void Deserialize(RawFile file, uint parentVersion)
        {
            // version < 0x1e has "unusedIsFixedUp"
            Type = file.ReadUInt(true);
            // version < 0x1f uses legacy array
            Instances = NuSerializer.ReadVectorArray<NuFaceOnInstance>(file);
        }

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandleUInt(ref Type);
            schema.HandleSchemaVector(ref Instances);
        }

        public void Serialize(RawFile file, uint parentVersion)
        {
            file.WriteUInt(Type, true);
            NuSerializer.WriteVectorArray(file, Instances);
        }
    }
}
