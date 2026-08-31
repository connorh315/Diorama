using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuTexGenHdr : ISchemaSerializable
    {
        public byte[] Checksum = new byte[16];
        public string Path = "";
        public short ResourceId;
        public const string DefaultName = "UnnamedImage";
        public string Name = DefaultName;
        public byte NutType;
        public byte Flags;

        public uint Level;
        public string ObjectId = "";
        public byte FixupType = 2;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandleArray(ref Checksum, 16);
            if (parentVersion < 0xc)
            {
                schema.HandleIntPascalString(ref Path, 1);
                schema.HandleShort(ref ResourceId);
            }
            else
            {
                schema.HandlePascalString(ref Path, 1);
                schema.HandlePascalString(ref Name, 1);
            }

            schema.HandleByte(ref NutType);

            if (parentVersion < 0xc)
            {
                schema.HandleByte(ref Flags);
            }

            if (parentVersion > 0xc)
            {
                schema.HandleUInt(ref Level);
                if (parentVersion > 0xd)
                {
                    schema.HandlePascalString(ref ObjectId, 1);
                }
                schema.HandleByte(ref FixupType);
            }
        }

        public void RandomiseChecksum()
        {
            new Random().NextBytes(Checksum);
        }
    }
}
