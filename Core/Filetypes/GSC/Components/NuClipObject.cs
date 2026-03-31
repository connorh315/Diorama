using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public struct NuClipItem
    {
        public int GeometryIndex;
        public int MaterialIndex;
    }

    public class NuClipObject : IVectorSerializable
    {
        public NuClipItem[] Elements;

        public void Deserialize(RawFile file, uint parentVersion)
        { // only works for DISP < 0x22
            ushort elementCount = file.ReadUShort(true);
            Elements = new NuClipItem[elementCount];
            for (int i = 0; i < elementCount; i++)
            {
                Elements[i] = new NuClipItem
                {
                    GeometryIndex = file.ReadInt(true),
                    MaterialIndex = file.ReadInt(true)
                };
            }
        }

        public void Serialize(RawFile file, uint parentVersion)
        {
            file.WriteUShort((ushort)Elements.Length, true);
            for (int i = 0; i < Elements.Length; i++)
            {
                file.WriteInt(Elements[i].GeometryIndex, true);
                file.WriteInt(Elements[i].MaterialIndex, true);
            }
        }
    }
}
