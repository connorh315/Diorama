using BrickVault;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuSceneInfo : ISchemaSerializable
    {
        private uint Version;

        private string Username;
        private string TimeDate;
        private string LegoPartId;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.Expect("OFNI");

            schema.HandleUInt(ref Version);

            if (Version < 2)
            {
                var ctx = (GSerializationContext)schema.Context;
                schema.HandleIntPascalString(ref Username, 1);
                if (!string.IsNullOrEmpty(Username))
                    ctx.AddReference(Username);
                schema.HandleIntPascalString(ref TimeDate, 1);
                if (!string.IsNullOrEmpty(TimeDate))
                    ctx.AddReference(TimeDate);
            }
            else
            {
                schema.HandlePascalString(ref Username);
                schema.HandlePascalString(ref TimeDate);
                if (Version > 2)
                {
                    schema.HandlePascalString(ref LegoPartId);
                }
            }
        }
    }
}
