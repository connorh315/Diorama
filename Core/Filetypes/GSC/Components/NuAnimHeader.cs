using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuAnimHeader : IVectorSerializable
    {
        public byte Version;

        public void Deserialize(RawFile file, uint parentVersion)
        {
            uint version = file.ReadUInt(true);
            //Debug.Assert(version.Substring(0, 3) == "ANI");
            Version = (byte)version;

            ushort numNodes = file.ReadUShort(true);
            ushort numFrames = file.ReadUShort(true);
            ushort curveGroupSize = file.ReadUShort(true);
            ushort originalNumFramesOld = file.ReadUShort(true);
            ushort numCurves = file.ReadUShort(true);
            ushort firstFrameOld = file.ReadUShort(true);
            byte endFrames = file.ReadByte();
            byte numShortIntegers = file.ReadByte();
            byte fixedUp = file.ReadByte();
            byte miscFlags = file.ReadByte();
            ushort totalNumFrames = file.ReadUShort(true);
            float constantBase = file.ReadFloat(true);
            float constantScale = file.ReadFloat(true);
            if ((miscFlags & 0x80) != 0)
            {
                float compressionRatio = file.ReadFloat(true);
                float firstFrame = file.ReadFloat(true);
            }

            uint keysNeeded = 0;
            if (Version <= 'A')
            {
                List<short> keyItems = NuSerializer.ReadLegacyVarArray<short>(file); // no keys needed
                //Debug.Assert(1 == 0, "ANIA unknown!");
            }
            else
            {
                keysNeeded = file.ReadUInt(true);
            }

            if (Version > 'C')
            {
                List<float> fConstants = NuSerializer.ReadLegacyVarArray<float>(file);
                uint bufferSize = file.ReadUInt(true); // not sure on this
                Debug.Assert(bufferSize == 0, "ANIC+ buffer size > 0 not seen before!");
            }

            List<short> constants = NuSerializer.ReadLegacyVarArray<short>(file);
            List<short> keyTypes = NuSerializer.ReadLegacyVarArray<short>(file);
            List<Ani3_ScaleMin> curveScaleMins = NuSerializer.ReadLegacyVarArray<Ani3_ScaleMin>(file);
            List<byte> tangentKeys = NuSerializer.ReadLegacyVarArray<byte>(file);
            List<byte> curveSetFlags = NuSerializer.ReadLegacyVarArray<byte>(file);

            if (Version > 'D')
            {
                byte[] buffer = file.ReadArray(file.ReadInt(true)); // 4-byte size prepended buffer
            }

            file.Seek(keysNeeded * curveGroupSize, SeekOrigin.Current);
        }
    }
}
