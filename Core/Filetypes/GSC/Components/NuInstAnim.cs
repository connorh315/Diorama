using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuInstAnim_4 : NuInstAnim
    {
        public override void ReadFlags(RawFile file)
        {
            ushort numframes = file.ReadUShort(true);
            uint numTextAnims = file.ReadUInt(true);
            ushort textureAnimListIdx = file.ReadUShort(true);
            ushort groupNode = file.ReadUShort(true);

            base.ReadFlags(file);
        }
    }

    public class NuInstAnim : IVectorSerializable
    {
        public NuMtx Mtx;

        public virtual void ReadFlags(RawFile file)
        {
            uint unk1 = file.ReadUInt(true); // TODO
            uint unk2 = file.ReadUInt(true);
            uint unk3 = file.ReadUInt(true);

            uint animIdx = file.ReadUInt(true);
            uint stateAnimIdx = file.ReadUInt(true);

            ushort instanceIdx = file.ReadUShort(true);
            ushort bsobj_ix = file.ReadUShort(true);
        }

        public void Deserialize(RawFile file, uint parentVersion)
        {
            Mtx = new NuMtx();
            Mtx.Deserialize(file, parentVersion);

            float tFactor = file.ReadFloat(true);
            float tFirst = file.ReadFloat(true);
            float tInterval = file.ReadFloat(true);
            float LocalTime = file.ReadFloat(true);

            ReadFlags(file);

            //byte[] bytes = file.ReadArray(34);

            //Console.WriteLine(BitConverter.ToString(bytes).Replace("-", " "));
        }

        public void Serialize(RawFile file, uint parentVersion)
        {
            throw new NotImplementedException();
        }
    }
}
