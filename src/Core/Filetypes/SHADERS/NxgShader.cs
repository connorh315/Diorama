using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.Core.Filetypes.SHADERS
{
    public class NxgShader : ISchemaSerializable
    {
        public uint Platform;
        public uint ConfigHash;
        public int VertexProgramHash;
        public int PixelProgramHash;

        public NxgShaderByteCode VertexProgram;
        public NxgShaderByteCode PixelProgram;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandleUInt(ref Platform);
            schema.HandleUInt(ref ConfigHash);
            schema.HandleInt(ref VertexProgramHash);
            schema.HandleInt(ref PixelProgramHash);

            schema.Handle(ref VertexProgram);
            schema.Handle(ref PixelProgram);
        }
    }
}
