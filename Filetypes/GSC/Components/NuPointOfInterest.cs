using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Filetypes.GSC.Components
{
    public class NuPointOfInterest : IVectorSerializable
    {
        public void Deserialize(RawFile file, uint parentVersion)
        {
            if (parentVersion < 0xb)
            {

            }
            else
            {
                string name = file.ReadPascalString();
            }

            NuMtx offset = new NuMtx();
            offset.Deserialize(file, parentVersion);

            byte parentJointIdx = file.ReadByte();
        }
    }
}
