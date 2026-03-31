using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuBlendCharShapeBlock
    {
        public uint Version;
        public List<NuBlendShapeAnimList> AnimList;

        public static NuBlendCharShapeBlock Parse(RawFile file)
        {
            NuBlendCharShapeBlock block = new NuBlendCharShapeBlock();
            Debug.Assert(file.ReadString(4) == "BCSB");

            block.Version = file.ReadUInt(true);
            block.AnimList = NuSerializer.ReadLegacyVarArray<NuBlendShapeAnimList>(file, block.Version);

            return block;
        }

        public void Write(RawFile file)
        {
            file.WriteString("BCSB");
            file.WriteUInt(Version, true);
            NuSerializer.WriteLegacyVarArray(file, AnimList, Version);
        }
    }
}
