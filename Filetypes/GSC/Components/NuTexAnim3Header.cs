using Diorama.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Filetypes.GSC.Components
{
    public class NuTexAnim3Header : IVectorSerializable
    {
        public void Deserialize(RawFile file)
        {
            uint mtlNameHash = file.ReadUInt(true);
            short animId = file.ReadShort(true);

            // Only for version != 3
            for (int i = 0; i < 8; i++)
            {
                short mtlIdx = file.ReadShort(true);
            }

            for (int i = 0; i < 8; i++)
            {
                byte mtlLayer = file.ReadByte();
            }

            for (int i = 0; i < 8; i++)
            {
                byte mtlUvSet = file.ReadByte();
            }

            List<short> tids = NuSerializer.ReadLegacyVarArray<short>(file);
            List<ushort> numTidsArray = NuSerializer.ReadLegacyVarArray<ushort>(file);
            List<NuCurveAnimBlock3> curveList = NuSerializer.ReadLegacyVarArray<NuCurveAnimBlock3>(file);

            uint one = file.ReadUInt(true);
            Debug.Assert(one == 1 || one == 0, "NuTexAnim3Header != 1 or 0!");
            if (one == 1)
            {
                NuAnimHeader header = new NuAnimHeader();
                header.Deserialize(file);
            }
        }
    }
}
