using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Filetypes.GSC.Components
{
    public class NuSpecialObject : IVectorSerializable
    {
        public void Deserialize(RawFile file)
        {
            string name = file.ReadPascalString(); // > 0x1b

            for (int i = 0; i < 16; i++)
                file.ReadFloat(); // matrix, no object for it yet

            Vector4 min = new Vector4(file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true));
            Vector4 max = new Vector4(file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true));
            Vector4 sphere = new Vector4(file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true));

            uint clipObjectIndex = file.ReadUInt(true);
            uint flags = file.ReadUInt(true);

            uint clipCount = file.ReadUInt(true); // this becomes a NuVector in 0x21
            for (int i = 0; i < clipCount; i++)
            {
                file.ReadFloat(true);
            }

            int instanceIndex = file.ReadInt(true);
            int animIndex = file.ReadInt(true);

            byte windSpeed = file.ReadByte();
            byte windScale = file.ReadByte();

            short nameIndex = file.ReadShort(true); // possibly actually "exported"?
        }
    }
}
