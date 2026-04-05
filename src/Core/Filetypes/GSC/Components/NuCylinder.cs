using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuCylinder : IVectorSerializable
    {
        public void Deserialize(RawFile file, uint parentVersion)
        {
            Vector3 centre = new Vector3(file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true));
            float xAxis = file.ReadFloat(true);
            Vector3 yAxis = new Vector3(file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true));
            float zAxis = file.ReadFloat(true);
        }

        public void Serialize(RawFile file, uint parentVersion)
        {
            throw new NotImplementedException();
        }
    }
}
