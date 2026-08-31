using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuOccluder : ISchemaSerializable
    {
        public string Name;
        public uint NameIndex;

        public NuMtx Mtx;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            if (parentVersion < 3)
            {
                schema.HandleUInt(ref NameIndex);
            }
            else
            {
                schema.HandlePascalString(ref Name);
            }

            schema.Handle(ref Mtx);
        }
    }
}
