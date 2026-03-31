using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuOccluderBlock
    {
        public uint Version;
        public List<ushort> NuOccluder;

        public static NuOccluderBlock Parse(RawFile file)
        {
            NuOccluderBlock block = new NuOccluderBlock();
            Debug.Assert(file.ReadString(4) == "BCCO");
            block.Version = file.ReadUInt(true);
            if (block.Version >= 3)
            {
                block.NuOccluder = NuSerializer.ReadVectorArray<ushort>(file);
                Debug.Assert(block.NuOccluder.Count == 0, "occluderlist != 0");
            }
            return block;
        }

        public void Write(RawFile file)
        {
            file.WriteString("BCCO");
            file.WriteUInt(Version, true);
            if (Version >= 3)
            {
                NuSerializer.WriteVectorArray(file, NuOccluder);
            }
        }
    }
}
