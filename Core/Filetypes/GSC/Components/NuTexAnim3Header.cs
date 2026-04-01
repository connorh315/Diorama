using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuTexAnim3Header : ISchemaSerializable
    {
        public uint MtlNameHash;
        public short AnimId;

        public short[] MtlIdx = new short[8];
        public byte[] MtlLayer = new byte[8];
        public byte[] MtlUvSet = new byte[8];

        public List<ushort> Tids;
        public List<ushort> NumTidsArray;
        public List<NuCurveAnimBlock3> CurveList;

        public NuAnimHeader AnimHeader = new NuAnimHeader();

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandleUInt(ref MtlNameHash);
            schema.HandleShort(ref AnimId);

            // When version != 3
            for (int i = 0; i < 8; i++)
            {
                schema.HandleShort(ref MtlIdx[i]);
            }

            for (int i = 0; i < 8; i++)
            {
                schema.HandleByte(ref MtlLayer[i]);
            }

            for (int i = 0; i < 8; i++)
            {
                schema.HandleByte(ref MtlUvSet[i]);
            }

            schema.HandleLegacyVarArray(ref Tids);
            schema.HandleLegacyVarArray(ref NumTidsArray);
            schema.HandleSchemaVarArray(ref CurveList);

            int hasAnimHeader = AnimHeader != null ? 1 : 0;
            schema.HandleInt(ref hasAnimHeader);

            if (hasAnimHeader == 1)
            {
                AnimHeader.Handle(schema, parentVersion);
            }
        }
    }
}
