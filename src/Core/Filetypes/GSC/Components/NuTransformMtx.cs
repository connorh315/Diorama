using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuTransformMtx : IVectorSerializable, ISchemaSerializable
    {
        public float[] Mtx = new float[12];

        public NuTransformMtx() { }

        public NuTransformMtx(Matrix4 mtx)
        {
            Update(mtx);
        }

        public void Update(Matrix4 mtx)
        {
            Mtx[0] = mtx.M11;
            Mtx[1] = mtx.M12;
            Mtx[2] = mtx.M13;
            Mtx[3] = mtx.M21;
            Mtx[4] = mtx.M22;
            Mtx[5] = mtx.M23;
            Mtx[6] = mtx.M31;
            Mtx[7] = mtx.M32;
            Mtx[8] = mtx.M33;
            Mtx[9] = mtx.M41;
            Mtx[10] = mtx.M42;
            Mtx[11] = mtx.M43;
        }

        public Matrix4 AsMatrix()
        {
            return new Matrix4(Mtx[0], Mtx[1], Mtx[2], 0, Mtx[3], Mtx[4], Mtx[5], 0, Mtx[6], Mtx[7], Mtx[8], 0, Mtx[9], Mtx[10], Mtx[11], 1);
        }

        public void Deserialize(RawFile file, uint parentVersion)
        {
            for (int i = 0; i < 12; i++)
                Mtx[i] = file.ReadFloat(true);
        }

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            for (int i = 0; i < 12; i++)
                schema.HandleFloat(ref Mtx[i]);
        }

        public void Serialize(RawFile file, uint parentVersion)
        {
            for (int i =0; i < 12; i++)
                file.WriteFloat(Mtx[i], true);
        }
    }
}
