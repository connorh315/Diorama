using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuMaterialDataBlock : ISchemaSerializable
    {
        public uint Version;

        public NuMaterialData[] Materials;

        public List<NuMtlOldReferencedMaterial> EmbeddedTextures;

        public List<NuDynamicString> ExternalMtlPaths;

        public int EngineHash;
        public int ShaderBuilderVersion;
        private byte HasShaderBlock;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.Expect("LTMU");
            schema.HandleUInt(ref Version);

            int count = schema.Writing ? Materials.Length : 0;
            schema.HandleInt(ref count);

            if (!schema.Writing)
            {
                Materials = new NuMaterialData[count];
                for (int i = 0; i < count; i++)
                {
                    Materials[i] = new NuMaterialData_E0();
                    Materials[i].Version = Version;
                    Materials[i].Handle(schema, Version);
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    Materials[i].Handle(schema, Version);
                }
            }

            if (Version < 0xfa)
            {
                schema.HandleSerializableVector(ref EmbeddedTextures);
            }

            if (Version < 0xf8)
            {
                schema.HandleByte(ref HasShaderBlock);
            }

            schema.HandleInt(ref EngineHash);
            schema.HandleInt(ref ShaderBuilderVersion);


            if (Version > 0xfc)
            {
                schema.HandleSerializableVector(ref ExternalMtlPaths);
            }
        }
    }
}
