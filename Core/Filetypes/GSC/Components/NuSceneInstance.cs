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
        public float FadeDistancesCo;
        public Vector3 FadeDistances;
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

            FadeDistancesCo = file.ReadFloat(true);
            FadeDistances = new Vector3(file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true));
            ApproxSize = file.ReadFloat(true);

            if ((Flags & 2) == 0)
            {
                FadeAlphaCo = file.ReadFloat(true);
                FadeAlpha = new Vector3(file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true));
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

            VertexControlledTint[0] = file.ReadInt(true);
            VertexControlledTint[1] = file.ReadInt(true);
            VertexControlledTint[2] = file.ReadInt(true);
            VertexControlledTint[3] = file.ReadInt(true);
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
