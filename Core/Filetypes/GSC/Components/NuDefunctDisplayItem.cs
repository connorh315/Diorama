using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public enum DisplayCommand
    {
        Material = 0x80,
        GeoCall = 0x82,
        Matrix = 0x83,
        Terminate = 0x84,
        MaterialClip = 0x85,
        Dummy = 0x87,
        DynamicGeo = 0x8b,
        End = 0x8e,
        FaceOn = 0x8f,
        LightMap = 0xb0,
        Mesh = 0xb3,
        Unknown2 = 0xb5,
        Other = 0x0
    }

    public class NuDefunctDisplayItem : IVectorSerializable
    {
        public DisplayCommand Command;
        public uint Index;

        public void Deserialize(RawFile file, uint parentVersion)
        {
            Command = (DisplayCommand)file.ReadByte();
            byte id = file.ReadByte();
            Index = file.ReadUInt(true);
        }
    }
}
