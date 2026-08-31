using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuPointOfInterest : ISchemaSerializable
    {
        public uint NameIndex;
        public string Name;

        public NuMtx Offset;

        public byte ParentJointIdx;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            if (parentVersion < 0xb)
            {
                schema.HandleUInt(ref NameIndex);
            }
            else
            {
                schema.HandlePascalString(ref Name);
            }
            schema.Handle(ref Offset);
            schema.HandleByte(ref ParentJointIdx);
        }
    }
}
