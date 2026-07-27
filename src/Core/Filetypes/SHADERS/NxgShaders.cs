using Diorama.Core.Filetypes.GSC.Components.RESH;
using Diorama.Rendering.Shaders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.Core.Filetypes.SHADERS
{
    public class NxgShaders : ISchemaSerializable
    {
        public static NxgShaders Read(RawFile file)
        {
            SchemaSerializer schema = new SchemaSerializer(file, false);

            NxgShaders shaders = new NxgShaders();

            shaders.Handle(schema, 0);

            return shaders;
        }

        public NuResourceHeader ResourceHeader;

        public int Version;
        public int Type;

        public int ShaderCacheVersion;

        public List<NxgShader> ShaderCache;

        public int TimeToWrite;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.Handle(ref ResourceHeader, 0);

            using (SchemaRegion region = new SchemaRegion(schema))
            {
                schema.Expect(".CC4HSERHSER");
                schema.HandleInt(ref Version);
                schema.HandleInt(ref Type);

                schema.Expect("HSCB");
                schema.HandleInt(ref ShaderCacheVersion);

                schema.HandleSchemaVarArray(ref ShaderCache, (uint)ShaderCacheVersion);

                schema.HandleInt(ref TimeToWrite);
            }
        }

        public NxgShader GetShader(uint hash)
        {
            foreach (var shader in ShaderCache)
            {
                if (shader.ConfigHash == hash) return shader;
            }

            return null;
        }
    }
}
