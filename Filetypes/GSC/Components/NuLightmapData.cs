using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Filetypes.GSC.Components
{
    public class NuLightmapData : IVectorSerializable
    {
        public void Deserialize(RawFile file, uint parentVersion)
        {
            uint type = file.ReadUInt(true);
            int meshInstanceId = file.ReadInt(true);
            int directionalTIDs0 = file.ReadInt(true);
            int directionalTIDs1 = file.ReadInt(true);
            int directionalTIDs2 = file.ReadInt(true);
            int smoothTID = file.ReadInt(true);
            int aoTID = file.ReadInt(true);

            float texCoordOffset0 = file.ReadFloat(true);
            float texCoordOffset1 = file.ReadFloat(true);
            float texCoordScale0 = file.ReadFloat(true);
            float texCoordScale1 = file.ReadFloat(true);
        }
    }
}
