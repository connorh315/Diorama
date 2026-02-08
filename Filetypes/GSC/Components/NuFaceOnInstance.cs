using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Filetypes.GSC.Components
{
    public class NuFaceOnInstance : IVectorSerializable
    {
        public void Deserialize(RawFile file)
        {
            Vector3 loc = new Vector3(file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true));
            float width = file.ReadFloat(true);
            float height = file.ReadFloat(true);
            uint colour = file.ReadUInt(true);
        }
    }
}
