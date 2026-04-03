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
        public string TextureName;
        public string Path;
        public byte NutType;

        public uint Level;
        public string ObjectId;
        public byte FixupType;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            parentVersion = 0xe; // not sure where this comes from

            schema.HandleArray(ref Checksum, 16);
            schema.HandlePascalString(ref TextureName, 1);
            schema.HandlePascalString(ref Path, 1);
            schema.HandleByte(ref NutType);

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
    }
}
