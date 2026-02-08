using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Filetypes.GSC.Components
{
    public class NuMtx : IVectorSerializable
    {
        public float[] mtx = new float[16];

        public void Deserialize(RawFile file, uint parentVersion)
        {
            for (int i = 0; i < 16; i++)
            {
                mtx[i] = file.ReadFloat(true);
            }
        }
    }
}
