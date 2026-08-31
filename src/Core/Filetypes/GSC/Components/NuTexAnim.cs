using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuTexAnim : ISchemaSerializable
    {
        public int LayerIndex;
        public int MaterialIndex;
        public int ScriptnameIndex;
        public int TidsIndex;
        public int NumTids;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandleInt(ref LayerIndex);
            schema.HandleInt(ref MaterialIndex);
            schema.HandleInt(ref ScriptnameIndex);
            schema.HandleInt(ref TidsIndex);
            schema.HandleInt(ref NumTids);
        }
    }
}
