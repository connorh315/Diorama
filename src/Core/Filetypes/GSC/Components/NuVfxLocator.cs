using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuVfxLocator : IVectorSerializable, ISchemaSerializable
    {
        public uint Version;

        public string VFXName;
        public string LEDFileName;

        public NuMtx Mtx;

        //public float[] MTX = new float[16];

        public float Radius;
        public byte FuzzySearch;
        public byte NxgOnly;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.Expect("LXFV");
            schema.HandleUInt(ref Version);

            schema.HandlePascalString(ref VFXName, 1);
            schema.HandlePascalString(ref LEDFileName, 1);

            schema.Handle(ref Mtx);

            if (Version > 1)
            {
                schema.HandleFloat(ref Radius);
                schema.HandleByte(ref FuzzySearch);
                schema.HandleByte(ref NxgOnly);
            }
        }

        public void Deserialize(RawFile file, uint parentVersion)
        {
            Debug.Assert(file.ReadString(4) == "LXFV");
            uint version = file.ReadUInt(true);

            VFXName = file.ReadPascalString();
            LEDFileName = file.ReadPascalString();

            Mtx = new NuMtx();
            Mtx.Deserialize(file, 0);

            //for (int i = 0; i < 16; i++)
            //{ // TODO: Matrix structure
            //    MTX[i] = file.ReadFloat(true);
            //}   

            if (version > 1)
            {
                Radius = file.ReadFloat(true);
                FuzzySearch = file.ReadByte();
                NxgOnly = file.ReadByte();
            }
        }

        public void Serialize(RawFile file, uint parentVersion)
        {
            throw new NotImplementedException();
        }
    }
}
