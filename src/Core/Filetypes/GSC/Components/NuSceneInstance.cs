using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuSceneInstance : IVectorSerializable
    {
        public int Hash;
        public short Flags;
        public short ClipObjectIndex;
        public float ClipDistance;
        public float[] FadeDistances = new float[4];
        public float ApproxSize;

        public float FadeAlphaCo;
        public Vector3 FadeAlpha;

        public NuSceneInstanceLod[] Lods;

        public int[] VertexControlledTint = new int[4];

        public void Deserialize(RawFile file, uint parentVersion)
        {
            Hash = file.ReadInt(true);
            Flags = file.ReadShort(true);
            ClipObjectIndex = file.ReadShort(true);
            //Console.WriteLine(clipObjectIndex);
            ClipDistance = file.ReadFloat(true);

            for (int i = 0; i < 4; i++)
            {
                FadeDistances[i] = file.ReadFloat(true);
            }

            ApproxSize = file.ReadFloat(true);

            if ((Flags & 2) == 0)
            {
                FadeAlphaCo = file.ReadFloat(true);
                FadeAlpha = file.ReadVector3(true);
            }
            else
            {
                Lods = new NuSceneInstanceLod[4];
                for (int lodId = 0; lodId < 4; lodId++)
                {
                    Lods[lodId] = new NuSceneInstanceLod
                    {
                        LodHeirarchical = file.ReadByte(),
                        HighResSceneFixupId = file.ReadInt(true),
                        FirstInstance = file.ReadInt(true),
                        NumInstances = file.ReadInt(true)
                    };
                }
            }

            if (parentVersion > 0x1f)
            {
                VertexControlledTint[0] = file.ReadInt(true);
                VertexControlledTint[1] = file.ReadInt(true);
                VertexControlledTint[2] = file.ReadInt(true);
                VertexControlledTint[3] = file.ReadInt(true);
            }
        }

        public void Serialize(RawFile file, uint parentVersion)
        {
            file.WriteInt(Hash, true);
            file.WriteShort(Flags, true);
            file.WriteShort(ClipObjectIndex, true);
            file.WriteFloat(ClipDistance, true);

            for (int i = 0; i < 4; i++)
            {
                file.WriteFloat(FadeDistances[i], true);
            }

            file.WriteFloat(ApproxSize, true);

            if ((Flags & 2) == 0)
            {
                file.WriteFloat(FadeAlphaCo, true);
                file.WriteVector3(FadeAlpha, true);
            }
            else
            {
                for (int lodId = 0; lodId < 4; lodId++)
                {
                    file.WriteByte(Lods[lodId].LodHeirarchical);
                    file.WriteInt(Lods[lodId].HighResSceneFixupId, true);
                    file.WriteInt(Lods[lodId].FirstInstance, true);
                    file.WriteInt(Lods[lodId].NumInstances, true);
                }
            }

            if (parentVersion > 0x1f)
            {
                for (int i = 0; i < 4; i++)
                {
                    file.WriteInt(VertexControlledTint[i], true);
                }
            }
        }
    }

    public struct NuSceneInstanceLod
    {
        public byte LodHeirarchical;
        public int HighResSceneFixupId;
        public int FirstInstance;
        public int NumInstances;
    }
}
