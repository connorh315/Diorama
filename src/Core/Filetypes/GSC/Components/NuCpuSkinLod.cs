using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuCpuSkinLod : IVectorSerializable, ISchemaSerializable
    {
        public List<NuCpuSkinLayer> Layers;
        public List<short> LayerIdTable;

        public void Deserialize(RawFile file, uint parentVersion)
        {
            if (parentVersion < 4)
            {

            }
            else
            {
                List<NuCpuSkinLayer> layers = NuSerializer.ReadVectorArray<NuCpuSkinLayer>(file, parentVersion);
                List<short> layerIdTable = NuSerializer.ReadVectorArray<short>(file);
            }
        }

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            if (parentVersion < 4)
            {
                Debug.Assert(1 == 0, "unsupported skin version!");
            }
            else
            {
                schema.HandleSchemaVector(ref Layers, parentVersion);
                schema.HandleSerializableVector(ref LayerIdTable);
            }
        }

        public void Serialize(RawFile file, uint parentVersion)
        {
            throw new NotImplementedException();
        }
    }
}
