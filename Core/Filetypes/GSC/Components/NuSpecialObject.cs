using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuSpecialObject : IVectorSerializable
    {
        public virtual List<float> ReadClipData(RawFile file)
        {
            return NuSerializer.ReadLegacyVarArray<float>(file);
        }

        public string Name;
        public NuMtx Mtx;

        public Vector4 Min;
        public Vector4 Max;
        public Vector4 Sphere;

        public uint ClipObjectIndex;
        public uint Flags;

        public List<float> ClipData;

        public int InstanceIndex;
        public int AnimIndex;

        public byte WindSpeed;
        public byte WindScale;

        public short NameIndex;

        public void Deserialize(RawFile file, uint parentVersion)
        {
            Name = file.ReadPascalString(); // > 0x1b

            Mtx = new NuMtx();
            Mtx.Deserialize(file, 0);

            Min = new Vector4(file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true));
            Max = new Vector4(file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true));
            Sphere = new Vector4(file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true));

            ClipObjectIndex = file.ReadUInt(true);
            Flags = file.ReadUInt(true);

            if (parentVersion == 0x21)
            {
                ClipData = NuSerializer.ReadVectorArray<float>(file);
            }
            else
            {
                ClipData = NuSerializer.ReadLegacyVarArray<float>(file);
            }

            InstanceIndex = file.ReadInt(true);
            AnimIndex = file.ReadInt(true);

            WindSpeed = file.ReadByte();
            WindScale = file.ReadByte();

            NameIndex = file.ReadShort(true); // possibly actually "exported"?
        }
    }
}
