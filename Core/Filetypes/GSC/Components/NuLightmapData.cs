using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuLightmapData : IVectorSerializable
    {
        public uint Type;
        public int MeshInstanceId;
        public int DirectionalTIDs0;
        public int DirectionalTIDs1;
        public int DirectionalTIDs2;
        public int SmoothTID;
        public int AoTID;

        public float TexCoordOffset0;
        public float TexCoordOffset1;
        public float TexCoordScale0;
        public float TexCoordScale1;


        public void Deserialize(RawFile file, uint parentVersion)
        {
            Type = file.ReadUInt(true);
            MeshInstanceId = file.ReadInt(true);
            DirectionalTIDs0 = file.ReadInt(true);
            DirectionalTIDs1 = file.ReadInt(true);
            DirectionalTIDs2 = file.ReadInt(true);
            SmoothTID = file.ReadInt(true);
            AoTID = file.ReadInt(true);

            TexCoordOffset0 = file.ReadFloat(true);
            TexCoordOffset1 = file.ReadFloat(true);
            TexCoordScale0 = file.ReadFloat(true);
            TexCoordScale1 = file.ReadFloat(true);
        }

        public void Serialize(RawFile file, uint parentVersion)
        {
            file.WriteUInt(Type, true);
            file.WriteInt(MeshInstanceId, true);
            file.WriteInt(DirectionalTIDs0, true);
            file.WriteInt(DirectionalTIDs1, true);
            file.WriteInt(DirectionalTIDs2, true);
            file.WriteInt(SmoothTID, true);
            file.WriteInt(AoTID, true);

            file.WriteFloat(TexCoordOffset0, true);
            file.WriteFloat(TexCoordOffset1, true);
            file.WriteFloat(TexCoordScale0, true);
            file.WriteFloat(TexCoordScale1, true);
        }
    }
}
