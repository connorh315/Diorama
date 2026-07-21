using Diorama.Core.Filetypes.GSC.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.Core.Filetypes.TEXTURES
{
    public class NuTextureSet : ISchemaSerializable
    {
        public uint Version;

        public string ConversionDate;

        public List<NuTexGenHdr> TextureHeaders;

        public NuTexture[] Textures;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.Expect("TSXT");
            schema.HandleUInt(ref Version);
            schema.HandleIntPascalString(ref ConversionDate, 1);
            schema.HandleSchemaVector(ref TextureHeaders, Version);
        }

        public void HandleImageContent(SchemaSerializer schema, uint parentVersion)
        {
            if (!schema.Writing)
            {
                Textures = new NuTexture[TextureHeaders.Count];
            }

            for (int i = 0; i < Textures.Length; i++)
            {
                if (!schema.Writing)
                {
                    Textures[i] = new NuTexture()
                    {
                        Header = TextureHeaders[i]
                    };
                }

                Textures[i].Handle(schema, Version);
            }
        }

        public NuTextureSet() { }

        public NuTextureSet(uint version, string conversionDate)
        {
            Version = version;
            ConversionDate = conversionDate;
        }
    }
}
