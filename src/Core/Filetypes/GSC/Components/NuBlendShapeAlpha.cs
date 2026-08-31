using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuBlendShapeAlpha : ISchemaSerializable
    {
        public float X;
        public float Y;
        public float Z;
        public float W;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandleFloat(ref X);
            schema.HandleFloat(ref Y);
            schema.HandleFloat(ref Z);
            schema.HandleFloat(ref W);
        }
    }
}
