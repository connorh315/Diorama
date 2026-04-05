using Diorama.Core.Types;
using SkiaSharp;
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

        private GSerializationContext ctx;

        public NuMaterialData_E0 HandleMaterial(SchemaSerializer schema)
        {
            int reference = 1;
            if (Version > 0xfd) // definitely exists in 0xfe
            {
                schema.HandleInt(ref reference);
            }

            if (reference == 1)
            {
                var mat = new NuMaterialData_E0();
                mat.Parent = this;
                mat.Version = Version;
                ctx.AddReference(mat);
                mat.Handle(schema, Version);


                return mat;
            }
            else if ((reference & 0xc0000000) != 0)
            {
                reference = (int)(reference & 0xffff);
                var mat = ctx.GetObject<NuMaterialData_E0>(reference);

                int shouldbe1 = 1;
                schema.HandleInt(ref shouldbe1);
                Debug.Assert(shouldbe1 == 1);

                return mat;
            }
            else if (reference == 0)
            {
                return null;
            }

            throw new Exception("wth");
        }

        public void HandleMaterialWrite(NuMaterialData mat, SchemaSerializer schema)
        {
            if (Version < 0x100)
            {
                mat.Handle(schema, Version);
                return;
            }

            int one = 1;
            if (ctx.GetOrAddReference(mat, out int reference))
            {
                reference = (int)(reference | 0xc0000000);
                schema.HandleInt(ref reference);
                schema.HandleInt(ref one);
            }
            else
            {
                if (reference == 0)
                {
                    schema.HandleInt(ref reference);
                    return;
                }

                schema.HandleInt(ref one);
                mat.Handle(schema, Version);
            }
        }

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            this.ctx = (GSerializationContext)schema.Context;

            schema.Expect("LTMU");
            schema.HandleUInt(ref Version);

            int count = schema.Writing ? Materials.Length : 0;
            schema.HandleInt(ref count);

            if (!schema.Writing)
            {
                Materials = new NuMaterialData[count];
                for (int i = 0; i < Materials.Length; i++)
                {
                    Materials[i] = HandleMaterial(schema);
                    //Materials[i] = new NuMaterialData_E0();
                    //Materials[i].Version = Version;
                    //Materials[i].Handle(schema, Version);
                }
            }
            else
            { // This is going to need some clever writing...
                for (int i = 0; i < count; i++)
                {
                    HandleMaterialWrite(Materials[i], schema);
                }
            }

            if (Version < 0xfa)
            {
                schema.HandleSchemaVector(ref EmbeddedTextures);
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
