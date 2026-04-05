using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuTextureHeader : IVectorSerializable, ISchemaSerializable
    {
        public byte[] Hash;
        public string Path;
        public string Name;
        public byte Type;

        public uint Level;
        public string ObjectId;
        public byte FixupTypeAsU8;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandleArray(ref Hash, 16);
            schema.HandlePascalString(ref Path, 1);
            schema.HandlePascalString(ref Name, 1);
            schema.HandleByte(ref Type);

            if (parentVersion > 0xc)
            {
                schema.HandleUInt(ref Level);
                if (parentVersion > 0xd)
                {
                    schema.HandlePascalString(ref ObjectId);
                }
                schema.HandleByte(ref FixupTypeAsU8);
            }
        }

        public void Deserialize(RawFile file, uint parentVersion)
        {
            Hash = file.ReadArray(16);
            Path = file.ReadPascalString();
            Name = file.ReadPascalString();
            Type = file.ReadByte();

            if (parentVersion > 0xc)
            {
                Level = file.ReadUInt(true);
                if (parentVersion > 0xd)
                {
                    ObjectId = file.ReadPascalString();
                }
                FixupTypeAsU8 = file.ReadByte();
            }
        }

        public void Serialize(RawFile file, uint parentVersion)
        {
            throw new NotImplementedException();
        }
    }
}
