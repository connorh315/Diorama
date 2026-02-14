using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuVfxLocator : IVectorSerializable
    {
        public string VFXName;
        public string LEDFileName;

        public float[] MTX = new float[16];

        public float Radius;
        public byte FuzzySearch;
        public byte NxgOnly;

        public void Deserialize(RawFile file, uint parentVersion)
        {
            Debug.Assert(file.ReadString(4) == "LXFV");
            uint version = file.ReadUInt(true);

            VFXName = file.ReadPascalString();
            LEDFileName = file.ReadPascalString();

            for (int i = 0; i < 16; i++)
            { // TODO: Matrix structure
                MTX[i] = file.ReadFloat(true);
            }   

            if (version > 1)
            {
                Radius = file.ReadFloat(true);
                FuzzySearch = file.ReadByte();
                NxgOnly = file.ReadByte();
            }
        }
    }
}
