using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuOctreeBlock
    {
        public uint Version;
        public List<NuLSVOctree> LSVOctree;

        public static NuOctreeBlock Parse(RawFile file)
        {
            NuOctreeBlock block = new NuOctreeBlock();
            Debug.Assert(file.ReadString(4) == "5LVI");
            block.Version = file.ReadUInt(true);
            if (block.Version >= 3)
            {
                block.LSVOctree = NuSerializer.ReadVectorArray<NuLSVOctree>(file);
                Debug.Assert(block.LSVOctree.Count == 0);
            }
            else
            {
                block.LSVOctree = NuSerializer.ReadLegacyVarArray<NuLSVOctree>(file);
            }
            return block;
        }

        public void Write(RawFile file)
        {
            file.WriteString("5LVI");
            file.WriteUInt(Version, true);
            if (Version >= 3)
            {
                NuSerializer.WriteVectorArray(file, LSVOctree);
            }
            else
            {
                NuSerializer.WriteLegacyVarArray(file, LSVOctree);
            }
        }
    }
}
