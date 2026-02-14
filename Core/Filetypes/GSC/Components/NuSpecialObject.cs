using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuSpecialObject_21 : NuSpecialObject
    {
        public override List<float> ReadClipData(RawFile file)
        {
            return NuSerializer.ReadVectorArray<float>(file);
        }
    }

    public class NuSpecialObject : IVectorSerializable
    {
        public virtual List<float> ReadClipData(RawFile file)
        {
            return NuSerializer.ReadLegacyVarArray<float>(file);
        }

        public void Deserialize(RawFile file, uint parentVersion)
        {
            string name = file.ReadPascalString(); // > 0x1b

            for (int i = 0; i < 16; i++)
                file.ReadFloat(); // matrix, no object for it yet

            Vector4 min = new Vector4(file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true));
            Vector4 max = new Vector4(file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true));
            Vector4 sphere = new Vector4(file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true));

            uint clipObjectIndex = file.ReadUInt(true);
            uint flags = file.ReadUInt(true);

            List<float> clipData = ReadClipData(file);

            int instanceIndex = file.ReadInt(true);
            int animIndex = file.ReadInt(true);

            byte windSpeed = file.ReadByte();
            byte windScale = file.ReadByte();

            short nameIndex = file.ReadShort(true); // possibly actually "exported"?
        }
    }
}
