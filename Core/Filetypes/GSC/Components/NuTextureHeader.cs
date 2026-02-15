using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuTextureHeader : IVectorSerializable
    {
        public byte[] Hash;
        public string Path;
        public string Name;
        public byte Type;

        public uint Level;
        public string ObjectId;
        public byte FixupTypeAsU8;

        public void Deserialize(RawFile file, uint parentVersion)
        {
            Hash = file.ReadArray(16);
            Path = file.ReadPascalString();
            Name = file.ReadPascalString();
            Type = file.ReadByte();

            if (parentVersion > 0xc)
            {
                Level = file.ReadUInt(true);
                if (parentVersion > 0xd)
                {
                    ObjectId = file.ReadPascalString();
                }
                FixupTypeAsU8 = file.ReadByte();
            }
        }
    }
}
