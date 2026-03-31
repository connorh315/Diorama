using Diorama.Core.Filetypes.GSC;
using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuBlendShape
    {
        public NuBlendShape Next;

        public static NuBlendShape Parse(RawFile file, GSerializationContext ctx, uint parentVersion)
        {
            var shape = new NuBlendShape();

            uint id = file.ReadUInt(true);

            ctx.AddReference(shape);

            uint nextShapeExists = file.ReadUInt(true);
            if (nextShapeExists != 0)
            {
                shape.Next = Parse(file, ctx, parentVersion);
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
                ctx.AddReference(buffer);
            }

            List<uint> runBatchTableV2 = NuSerializer.ReadVectorArray<uint>(file);

            return shape;
        }
    }
}
