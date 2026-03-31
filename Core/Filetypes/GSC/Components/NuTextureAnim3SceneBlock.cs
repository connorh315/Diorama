using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuTextureAnim3SceneBlock
    {
        public uint Version;
        public List<NuTexAnim3Header> TexAnim3Headers;

        public static NuTextureAnim3SceneBlock Parse(RawFile file)
        {
            NuTextureAnim3SceneBlock block = new NuTextureAnim3SceneBlock();

            Debug.Assert(file.ReadString(4) == "BNAT");
            block.Version = file.ReadUInt(true);
            Debug.Assert(block.Version == 5);

            block.TexAnim3Headers = NuSerializer.ReadLegacyVarArray<NuTexAnim3Header>(file);

            return block;
        }

        public void Write(RawFile file)
        {
            file.WriteString("BNAT");
            file.WriteUInt(Version, true);
            NuSerializer.WriteLegacyVarArray(file, TexAnim3Headers);
        }
    }
}
