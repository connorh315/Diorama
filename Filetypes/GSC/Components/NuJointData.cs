using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Filetypes.GSC.Components
{
    public class NuJointData : IVectorSerializable
    {
        public void Deserialize(RawFile file, uint parentVersion)
        {
            if (parentVersion < 0xd)
            {
                uint nameIndex = file.ReadUInt(true);
            }
            else
            {
                string name = file.ReadPascalString();
            }

            NuMtx orient = new NuMtx();
            orient.Deserialize(file, parentVersion);
            Vector3 locatorOffset = new Vector3(file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true));

            byte parentIndex = file.ReadByte();
            byte flags = file.ReadByte();
        }
    }
}
