using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuIrradianceBlock : ISchemaSerializable
    {
        public int Exists = 1;

        public uint Version;

        public List<ushort> Boxes;
        public List<ushort> Planes;
        public List<ushort> PlaneLayers;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandleInt(ref Exists);
            if (Exists == 0)
            {
                return;
            }

            schema.Expect("LOVI");
            schema.HandleUInt(ref Version);

            if (Version < 7)
            {
                schema.HandleLegacyVarArray(ref Boxes);
                schema.HandleLegacyVarArray(ref Planes);
                schema.HandleLegacyVarArray(ref PlaneLayers);
            }
            else
            {
                Debug.Assert(1 == 0, "vector section of illuminance block!");
                //schema.HandleShort()
            }
        }
    }
}
