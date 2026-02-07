using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Filetypes.GSC.Components
{
    public class NuInstAnim : IVectorSerializable
    {
        public NuMtx Mtx;

        public void Deserialize(RawFile file)
        {
            Mtx = new NuMtx();
            Mtx.Deserialize(file);

            float tFactor = file.ReadFloat(true);
            float tFirst = file.ReadFloat(true);
            float tInterval = file.ReadFloat(true);
            float LocalTime = file.ReadFloat(true);

            uint unk1 = file.ReadUInt(true); // TODO
            uint unk2 = file.ReadUInt(true);
            uint unk3 = file.ReadUInt(true);

            uint animIdx = file.ReadUInt(true);
            uint stateAnimIdx = file.ReadUInt(true);

            ushort instanceIdx = file.ReadUShort(true);
            ushort bsobj_ix = file.ReadUShort(true);
        }
    }
}
