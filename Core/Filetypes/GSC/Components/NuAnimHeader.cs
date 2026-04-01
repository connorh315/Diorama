using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuAnimHeader : ISchemaSerializable
    {
        public uint Version;

        public ushort NumNodes;
        public ushort NumFrames;
        public ushort CurveGroupSize;
        public ushort OriginalNumFramesOld;
        public ushort NumCurves;
        public ushort FirstFrameOld;
        public byte EndFrames;
        public byte NumShortIntegers;
        public byte FixedUp;
        public byte MiscFlags;
        public ushort TotalNumFrames;
        public float ConstantBase;
        public float ConstantScale;

        public float CompressionRatio;
        public float FirstFrame;

        public uint KeysNeeded = 0;

        public List<ushort> KeyItems;

        public List<float> FConstants;
        public int BufferSize;

        public List<ushort> Constants;
        public List<ushort> KeyTypes;
        public List<Ani3_ScaleMin> CurveScaleMins;
        public List<byte> TangentKeys;
        public List<byte> CurveSetFlags;

        public byte[] Buffer;

        public byte[] CurveGroupKeys;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandleUInt(ref Version);
            byte major = (byte)(Version);

            schema.HandleUShort(ref NumNodes);
            schema.HandleUShort(ref NumFrames);
            schema.HandleUShort(ref CurveGroupSize);
            schema.HandleUShort(ref OriginalNumFramesOld);
            schema.HandleUShort(ref NumCurves);
            schema.HandleUShort(ref FirstFrameOld);
            schema.HandleByte(ref EndFrames);
            schema.HandleByte(ref NumShortIntegers);
            schema.HandleByte(ref FixedUp);
            schema.HandleByte(ref MiscFlags);
            schema.HandleUShort(ref TotalNumFrames);
            schema.HandleFloat(ref ConstantBase);
            schema.HandleFloat(ref ConstantScale);
            
            if ((MiscFlags & 0x80) != 0)
            {
                schema.HandleFloat(ref CompressionRatio);
                schema.HandleFloat(ref FirstFrame);
            }

            if (major <= 'A')
            {
                schema.HandleLegacyVarArray(ref KeyItems);
                //Debug.Assert(1 == 0, "ANIA unknown!");
            }
            else
            {
                schema.HandleUInt(ref KeysNeeded);
            }

            if (major > 'C')
            {
                schema.HandleLegacyVarArray(ref FConstants);
                schema.HandleInt(ref BufferSize);

                Debug.Assert(BufferSize == 0, "ANIC+ buffer size > 0 not seen before!");
            }

            schema.HandleLegacyVarArray(ref Constants);
            schema.HandleLegacyVarArray(ref KeyTypes);
            schema.HandleSchemaVarArray(ref CurveScaleMins);
            schema.HandleLegacyVarArray(ref TangentKeys);
            schema.HandleLegacyVarArray(ref CurveSetFlags);

            if (major > 'D')
            {
                int bufferSize = Buffer.Length;
                schema.HandleInt(ref bufferSize);
                schema.HandleArray(ref Buffer, bufferSize);
            }

            schema.HandleArray(ref CurveGroupKeys, (int)(KeysNeeded * CurveGroupSize));
        }
    }
}
