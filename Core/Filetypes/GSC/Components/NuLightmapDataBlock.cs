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

        public static NuLightmapDataBlock Parse(RawFile file)
        {
            NuLightmapDataBlock block = new NuLightmapDataBlock();

            Debug.Assert(file.ReadString(4) == "TDML");
            block.Version = file.ReadUInt(true);
            if (block.Version >= 3)
            {
                block.Lightmaps = NuSerializer.ReadVectorArray<NuLightmapData>(file);
            }

            return block;
        }

        public void Write(RawFile file)
        {
            file.WriteString("TDML");
            file.WriteUInt(Version, true);
            if (Version >= 3)
            {
                NuSerializer.WriteVectorArray(file, Lightmaps);
            }
        }
    }
}
