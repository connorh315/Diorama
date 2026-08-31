using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuRoomPortals : ISchemaSerializable
    {
        public uint Version;

        public List<NuPortal> Portals;
        public List<NuRoom> Rooms;
        public List<ushort> Instances;
        public List<Vector4> Planes;
        public List<NuVec> Points;
        public List<short> PortalIndex;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.Expect("TROP");
            schema.HandleUInt(ref Version);

            schema.HandleSchemaVarArray(ref Portals);
            schema.HandleSchemaVarArray(ref Rooms);
            schema.HandleLegacyVarArray(ref Instances);
            schema.HandleLegacyVarArray(ref Planes);
            schema.HandleSchemaVarArray(ref Points);
            schema.HandleLegacyVarArray(ref PortalIndex);
        }
    }
}
