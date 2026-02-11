using Diorama.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Filetypes.GSC.Components
{
    public class NuBlendShape
    {
        public NuBlendShape Next;

        public static NuBlendShape Parse(RawFile file, GScene scene, uint parentVersion)
        {
            var shape = new NuBlendShape();

            uint id = file.ReadUInt(true);

            scene.referenceCounter += 1;

            uint nextShapeExists = file.ReadUInt(true);
            if (nextShapeExists != 0)
            {
                shape.Next = NuBlendShape.Parse(file, scene, parentVersion);
            }

            if (parentVersion < 0xae)
            {
                List<NuVec> offsets = NuSerializer.ReadLegacyVarArray<NuVec>(file);
            }
            else
            {
                List<NuVec> offsets = NuSerializer.ReadVectorArray<NuVec>(file);
            }

            if (parentVersion < 0xae)
            {
                Debug.Assert(1 == 0, "NuBlendShape section not implemented");
            }

            uint compressionFormat = file.ReadUInt(true);
            int bufferSize = file.ReadInt(true);
            byte[] buffer = file.ReadArray(bufferSize);
            if (bufferSize != 0)
            {
                scene.referenceCounter++;
            }

            List<uint> runBatchTableV2 = NuSerializer.ReadVectorArray<uint>(file);

            return shape;
        }
    }
}
