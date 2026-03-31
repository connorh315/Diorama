using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuCurveAnimBlock3 : IVectorSerializable
    {
        public void Deserialize(RawFile file, uint parentVersion)
        {
            int iResult = file.ReadInt(true);
            short firstFrame = file.ReadShort(true);
            short numFrames = file.ReadShort(true);
            byte animActive = file.ReadByte();
            byte postAnimCycling = file.ReadByte();
            byte preAnimCycling = file.ReadByte();
            byte updateFlag = file.ReadByte();
            byte attributeId = file.ReadByte();
            
            // if version > 4
            byte usesSpriteSheet = file.ReadByte();
            byte SSFaceon = file.ReadByte();

            // no names on these 4 bytes
            byte undefined1 = file.ReadByte();
            byte undefined2 = file.ReadByte();
            byte undefined3 = file.ReadByte();
            byte undefined4 = file.ReadByte();
        }

        public void Serialize(RawFile file, uint parentVersion)
        {
            throw new NotImplementedException();
        }
    }
}
