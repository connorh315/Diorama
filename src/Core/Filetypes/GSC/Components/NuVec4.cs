using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuVec4 : IVectorSerializable, ISchemaSerializable
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

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandleFloat(ref X);
            schema.HandleFloat(ref Y);
            schema.HandleFloat(ref Z);
            schema.HandleFloat(ref W);
        }

        public void Serialize(RawFile file, uint parentVersion)
        {
            file.WriteFloat(X, true);
            file.WriteFloat(Y, true);
            file.WriteFloat(Z, true);
            file.WriteFloat(W, true);
        }
    }
}
