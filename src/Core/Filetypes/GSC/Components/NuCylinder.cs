using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuCylinder : IVectorSerializable, ISchemaSerializable
    {
        public Vector3 Centre;
        public float XAxis;
        public Vector3 YAxis;
        public float ZAxis;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandleVector3(ref Centre);
            schema.HandleFloat(ref XAxis);
            schema.HandleVector3(ref YAxis);
            schema.HandleFloat(ref ZAxis);
        }

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
