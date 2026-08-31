using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuRoomInst : ISchemaSerializable
    {
        public List<ushort> Instances;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandleLegacyVarArray(ref Instances);
        }
    }
}
