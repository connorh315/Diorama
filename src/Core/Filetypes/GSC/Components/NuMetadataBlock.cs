using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuMetadataBlock
    {
        public uint Version;
        public List<NuDynamicString> MetaStrings;

        public static NuMetadataBlock Parse(RawFile file)
        {
            NuMetadataBlock block = new NuMetadataBlock();
            Debug.Assert(file.ReadString(4) == "ATEM");
            block.Version = file.ReadUInt(true);

            if (block.Version >= 0x46)
            {
                block.MetaStrings = NuSerializer.ReadVectorArray<NuDynamicString>(file);
            }
            return block;
        }

        public void Write(RawFile file)
        {
            file.WriteString("ATEM");
            file.WriteUInt(Version, true);
            if (Version >= 0x46)
            {
                NuSerializer.WriteVectorArray(file, MetaStrings);
            }
        }
    }
}
