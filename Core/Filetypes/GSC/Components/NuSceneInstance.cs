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
        public void Deserialize(RawFile file, uint parentVersion)
        {
            int hash = file.ReadInt(true);
            short flags = file.ReadShort(true);
            short clipObjectIndex = file.ReadShort(true);
            Console.WriteLine(clipObjectIndex);
            float clipDistance = file.ReadFloat(true);

            float fadeDistancesCo = file.ReadFloat(true);
            Vector3 fadeDistances = new Vector3(file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true));
            float approxSize = file.ReadFloat(true);

            if ((flags & 2) == 0)
            {
                float fadeAlphaCo = file.ReadFloat(true);
                Vector3 fadeAlpha = new Vector3(file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true));
            }
            else
            {
                for (int lodId = 0; lodId < 4; lodId++)
                {
                    byte lodHeirarchical = file.ReadByte();
                    int highResSceneFixupId = file.ReadInt(true);
                    int firstInstance = file.ReadInt(true);
                    int numInstances = file.ReadInt(true);
                }
            }

            int vertexControlledTint0 = file.ReadInt(true);
            int vertexControlledTint1 = file.ReadInt(true);
            int vertexControlledTint2 = file.ReadInt(true);
            int vertexControlledTint3 = file.ReadInt(true);
        }
    }
}
