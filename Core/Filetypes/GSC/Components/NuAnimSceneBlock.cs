using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuAnimSceneBlock
    {
        public uint Version;
        public List<NuInstAnim> NuInstAnim;
        public List<NuStateAnim> NuStateAnim;
        public List<NuAnimHeader> NuAnimHeader;

        public static NuAnimSceneBlock Parse(RawFile file)
        {
            NuAnimSceneBlock block = new NuAnimSceneBlock();

            Debug.Assert(file.ReadString(4) == "3ALA");
            block.Version = file.ReadUInt(true);
            Debug.Assert(block.Version == 3 || block.Version == 4, "ala3 vesrion!");

            if (block.Version == 3)
            {
                block.NuInstAnim = NuSerializer.ReadLegacyVarArray<NuInstAnim>(file); // 1wizardofozc2_tech_dx11.gsc
            }
            else if (block.Version == 4)
            {
                List<NuInstAnim_4> nuinstanim = NuSerializer.ReadLegacyVarArray<NuInstAnim_4>(file); // TODO: Implement
            }

            block.NuStateAnim = NuSerializer.ReadLegacyVarArray<NuStateAnim>(file);

            block.NuAnimHeader = NuSerializer.ReadLegacyVarArray<NuAnimHeader>(file);

            return block;
        }

        public void Write(RawFile file)
        {
            file.WriteString("3ALA");
            file.WriteUInt(Version, true);
            if (Version == 3)
            {
                NuSerializer.WriteLegacyVarArray(file, NuInstAnim);
            }
            else if (Version == 4)
            {
                throw new NotImplementedException("Version 4 NuInstAnim writing not implemented yet");
            }

            NuSerializer.WriteLegacyVarArray(file, NuStateAnim);
            NuSerializer.WriteLegacyVarArray(file, NuAnimHeader);
        }
    }
}
