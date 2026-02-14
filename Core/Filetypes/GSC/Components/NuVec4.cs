using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuVec4 : IVectorSerializable
    {
        public float X;
        public float Y;
        public float Z;
        public float W;

        public void Deserialize(RawFile file, uint parentVersion)
        {
            X = file.ReadFloat(true);
            Y = file.ReadFloat(true);
            Z = file.ReadFloat(true);
            W = file.ReadFloat(true);
        }
    }
}
