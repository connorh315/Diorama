using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuTransformMtx : IVectorSerializable
    {
        public float[] Mtx = new float[12];

        public void Deserialize(RawFile file, uint parentVersion)
        {
            for (int i = 0; i < 12; i++)
                Mtx[i] = file.ReadFloat(true);
        }

        public void Serialize(RawFile file, uint parentVersion)
        {
            for (int i =0; i < 12; i++)
                file.WriteFloat(Mtx[i], true);
        }
    }
}
