using Diorama.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Filetypes.GSC.Components
{
    public class NuStateAnim : IVectorSerializable
    {
        public void Deserialize(RawFile file)
        {
            ushort endFrame = file.ReadUShort(true);
            List<float> frames = NuSerializer.ReadLegacyVarArray<float>(file);
            List<byte> states = NuSerializer.ReadLegacyVarArray<byte>(file);
        }
    }
}
