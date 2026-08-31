using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuIrradiancePlaneSample : ISchemaSerializable
    {
        public int Direction0;
        public int Direction1;
        public int Direction2;
        public int Ambient;
        public byte Occlusion;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandleInt(ref Direction0);
            schema.HandleInt(ref Direction1);
            schema.HandleInt(ref Direction2);
            schema.HandleInt(ref Ambient);
            schema.HandleByte(ref Occlusion);
        }
    }
}
