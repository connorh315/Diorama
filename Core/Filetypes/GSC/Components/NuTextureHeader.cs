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

        public void Deserialize(RawFile file, uint parentVersion)
        {
            Hash = file.ReadArray(16);
            Path = file.ReadPascalString();
            Name = file.ReadPascalString();
            Type = file.ReadByte();
        }
    }
}
