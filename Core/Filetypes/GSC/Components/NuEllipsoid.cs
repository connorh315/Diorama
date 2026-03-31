using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuEllipsoid : IVectorSerializable
    {
        public void Deserialize(RawFile file, uint parentVersion)
        {
            Vector3 centre = new Vector3(file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true));
            Vector3 x_axis = new Vector3(file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true));
            Vector3 y_axis = new Vector3(file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true));
            Vector3 z_axis = new Vector3(file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true));
        }

        public void Serialize(RawFile file, uint parentVersion)
        {
            throw new NotImplementedException();
        }
    }
}
