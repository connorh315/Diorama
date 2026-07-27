using Diorama.Core.Filetypes.GSC.Components;
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

        public byte IsGraphShader;

        public List<NuDynamicString> UserTextureNames0;
        public List<NuDynamicString> UserTextureNames1;

        public NxgShaderByteCode VertexProgram;
        public NxgShaderByteCode PixelProgram;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandleUInt(ref Platform);
            schema.HandleUInt(ref ConfigHash);
            schema.HandleInt(ref VertexProgramHash);
            schema.HandleInt(ref PixelProgramHash);
            if (parentVersion > 0xc)
            {
                schema.HandleByte(ref IsGraphShader);

                if (parentVersion > 0xf)
                {
                    schema.HandleSchemaVector(ref UserTextureNames0);
                }
                schema.HandleSchemaVector(ref UserTextureNames1);
            }

            schema.Handle(ref VertexProgram);
            schema.Handle(ref PixelProgram);
        }
    }
}
