using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuLSVSample : IVectorSerializable
    {
        public void Deserialize(RawFile file, uint parentVersion)
        {
            int lightId1 = file.ReadInt(true);
            float shadowFactor1 = file.ReadFloat(true);

            int lightId2 = file.ReadInt(true);
            float shadowFactor2 = file.ReadFloat(true);

            for (int i= 0; i < 9; i++)
            {
                Vector3 vec = new Vector3(file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true));
            }
        }

        public void Serialize(RawFile file, uint parentVersion)
        {
            throw new NotImplementedException();
        }
    }
}
