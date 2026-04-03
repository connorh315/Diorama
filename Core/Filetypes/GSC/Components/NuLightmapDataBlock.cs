using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuLightmapDataBlock
    {
        public uint Version { get; set; }

        public List<NuLightmapData> Lightmaps { get; set; }

        public float MinU;
        public float MaxU;
        public float MinV;
        public float MaxV;

        public static NuLightmapDataBlock Parse(RawFile file)
        {
            NuLightmapDataBlock block = new NuLightmapDataBlock();

            Debug.Assert(file.ReadString(4) == "TDML");
            block.Version = file.ReadUInt(true);
            if (block.Version >= 3)
            {
                block.Lightmaps = NuSerializer.ReadVectorArray<NuLightmapData>(file, block.Version);
            }

            if (block.Version > 0xd)
            {
                block.MinU = file.ReadFloat(true);
                block.MaxU = file.ReadFloat(true);
                block.MinV = file.ReadFloat(true);
                block.MaxV = file.ReadFloat(true);
            }

            return block;
        }

        public void Write(RawFile file)
        {
            file.WriteString("TDML");
            file.WriteUInt(Version, true);
            if (Version >= 3)
            {
                NuSerializer.WriteVectorArray(file, Lightmaps, Version);
            }

            if (Version > 0xd)
            {
                file.WriteFloat(MinU, true);
                file.WriteFloat(MaxU, true);
                file.WriteFloat(MinV, true);
                file.WriteFloat(MaxV, true);
            }
        }
    }
}
