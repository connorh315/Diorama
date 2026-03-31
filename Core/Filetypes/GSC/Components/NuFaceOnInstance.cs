using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuFaceOnInstance : IVectorSerializable
    {
        public Vector3 Loc;
        public float Width;
        public float Height;
        public uint Colour;

        public void Deserialize(RawFile file, uint parentVersion)
        {
            Loc = file.ReadVector3(true);
            Width = file.ReadFloat(true);
            Height = file.ReadFloat(true);
            Colour = file.ReadUInt(true);
        }

        public void Serialize(RawFile file, uint parentVersion)
        {
            file.WriteVector3(Loc, true);
            file.WriteFloat(Width, true);
            file.WriteFloat(Height, true);
            file.WriteUInt(Colour, true);
        }
    }
}
