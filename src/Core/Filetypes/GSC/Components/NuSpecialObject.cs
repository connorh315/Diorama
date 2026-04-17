using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuSpecialObject : IVectorSerializable, ISchemaSerializable
    {
        public string Name;
        public NuMtx Mtx;

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

        public short NameIndex;

        public void Deserialize(RawFile file, uint parentVersion)
        {
            Name = file.ReadPascalString(); // > 0x1b

            Mtx = new NuMtx();
            Mtx.Deserialize(file, 0);

            Min = file.ReadVector4(true);
            Max = file.ReadVector4(true);
            Sphere = file.ReadVector4(true);

            ClipObjectIndex = file.ReadUInt(true);
            Flags = file.ReadUInt(true);

            if (parentVersion > 0x20)
            {
                ClipData = NuSerializer.ReadVectorArray<float>(file);
            }
            else
            {
                ClipData = NuSerializer.ReadLegacyVarArray<float>(file);
            }

            InstanceIndex = file.ReadInt(true);
            AnimIndex = file.ReadInt(true);

            WindSpeed = file.ReadByte();
            WindScale = file.ReadByte();

            NameIndex = file.ReadShort(true); // possibly actually "exported"?
        }

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandlePascalString(ref Name, 1);
            schema.Handle(ref Mtx);
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

            schema.HandleShort(ref NameIndex);
        }

        public void Serialize(RawFile file, uint parentVersion)
        {
            file.WritePascalString(Name, 1);

            Mtx.Serialize(file, 0);

            file.WriteVector4(Min, true);
            file.WriteVector4(Max, true);
            file.WriteVector4(Sphere, true);

            file.WriteUInt(ClipObjectIndex, true);
            file.WriteUInt(Flags, true);

            if (parentVersion > 0x20)
            {
                NuSerializer.WriteVectorArray(file, ClipData, 0);
            }
            else
            {
                NuSerializer.WriteLegacyVarArray(file, ClipData, 0);
            }

            file.WriteInt(InstanceIndex, true);
            file.WriteInt(AnimIndex, true);

            file.WriteByte(WindSpeed);
            file.WriteByte(WindScale);

            file.WriteShort(NameIndex, true);
        }


    }
}
