using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuPortal : ISchemaSerializable
    {
        public int PointsIndex;
        public short NumPoints;
        public Vector4 PlaneEqn;
        public short LeftRoom;
        public short RightRoom;
        public byte Id;
        public int Flags;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandleInt(ref PointsIndex);
            schema.HandleShort(ref NumPoints);
            schema.HandleVector4(ref PlaneEqn);
            schema.HandleShort(ref LeftRoom);
            schema.HandleShort(ref RightRoom);
            schema.HandleByte(ref Id);
            schema.HandleInt(ref Flags);
        }
    }
}
