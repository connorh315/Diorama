using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.Core.Filetypes.SHADERS
{
    public class NxgShaderByteCode : ISchemaSerializable
    {
        public uint Platform;
        public int BytecodeHash;

        public byte[] Program;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandleUInt(ref Platform);
            schema.HandleInt(ref BytecodeHash);

            schema.HandleBuffer(ref Program);
        }
    }
}
