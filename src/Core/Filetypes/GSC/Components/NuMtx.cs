using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuMtx : IVectorSerializable, ISchemaSerializable
    {
        public float[] mtx = new float[16];

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            for (int i = 0; i < 16; i++)
            {
                schema.HandleFloat(ref mtx[i]);
            }
        }

        public void Deserialize(RawFile file, uint parentVersion)
        {
            for (int i = 0; i < 16; i++)
            {
                mtx[i] = file.ReadFloat(true);
            }
        }

        public void Serialize(RawFile file, uint parentVersion)
        {
            for (int i = 0; i < 16; i++)
            {
                file.WriteFloat(mtx[i], true);
            }
        }
    }
}
