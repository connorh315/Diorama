using BrickVault;
using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuTextureHeaders : ISchemaSerializable
    {
        public uint Version;

        public List<uint> Unk;

        public List<NuTextureHeader> Headers;

        public List<uint> Unk2;

        public static NuTextureHeaders Read(RawFile file)
        {
            NuTextureHeaders headers = new NuTextureHeaders();
            
            Debug.Assert(file.ReadString(4) == "HGXT");
            Debug.Assert(file.ReadUInt(true) == 0xc);

            headers.Headers = NuSerializer.ReadVectorArray<NuTextureHeader>(file);

            return headers;
        }

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.Expect("HGXT");
            schema.HandleUInt(ref Version);

            var ctx = (GSerializationContext)schema.Context;

            if (Version < 0xb)
            {
                schema.HandleSerializableVector(ref Unk);
                //ctx.AddReference(Unk);
            }

            schema.HandleSchemaVector(ref Headers, Version);
            //ctx.AddReference(Headers);

            //foreach (var header in Headers)
            //{
            //    ctx.AddReference(header);
            //}

            if (Version < 0xa)
            {
                schema.HandleSerializableVector(ref Unk2);
                ctx.AddReference(Unk2);
            }
        }
    }
}
