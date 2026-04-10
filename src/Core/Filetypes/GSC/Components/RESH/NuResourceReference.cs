using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components.RESH
{
    public class NuResourceReference : ISchemaSerializable
    {
        public uint Type;
        public int OldParam;
        public uint Hash;
        public NuCheckSum Checksum;
        public uint PlatformsAndClasses;
        public uint ForContext;
        public uint WithContext;
        public int Discipline;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandleUInt(ref Type);
            if (parentVersion < 7)
            {
                schema.HandleInt(ref OldParam);
            }
            schema.HandleUInt(ref Hash);
            schema.HandleOptional(ref Checksum);
            if (parentVersion > 2)
            {
                schema.HandleUInt(ref PlatformsAndClasses);
                if (parentVersion > 5)
                {
                    schema.HandleUInt(ref ForContext);
                    schema.HandleUInt(ref WithContext);
                    if (parentVersion > 7)
                    {
                        schema.HandleInt(ref Discipline);
                    }
                }
            }
        }
    }
}
