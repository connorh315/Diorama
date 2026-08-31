using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuSpecialObject : ISchemaSerializable
    {
        public string Name;
        public uint NameIndex;
        public NuMtx Mtx;
        public NuMtx DrawMtx;

        public Vector4 Min;
        public Vector4 Max;
        public Vector4 Sphere;

        public uint ClipObjectIndex;
        public uint Flags;

        public List<float> ClipData;

        public int InstanceIndex;
        public int AnimIndex;

        public byte WindSpeed;
        public byte WindScale;

        public short exported;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            if (parentVersion < 0x1b)
            {
                schema.HandleUInt(ref NameIndex);
            }
            else
            {
                schema.HandlePascalString(ref Name, 1);
            }
            schema.Handle(ref Mtx);
            if (parentVersion < 0x1c)
            {
                schema.Handle(ref DrawMtx);
            }
            schema.HandleVector4(ref Min);
            schema.HandleVector4(ref Max);
            schema.HandleVector4(ref Sphere);

            schema.HandleUInt(ref ClipObjectIndex);
            schema.HandleUInt(ref Flags);

            if (parentVersion > 0x20)
            {
                schema.HandleSerializableVector(ref ClipData);
            }
            else
            {
                schema.HandleLegacyVarArray(ref ClipData);
            }

            schema.HandleInt(ref InstanceIndex);
            schema.HandleInt(ref AnimIndex);

            schema.HandleByte(ref WindSpeed);
            schema.HandleByte(ref WindScale);

            schema.HandleShort(ref exported);
        }
    }
}
