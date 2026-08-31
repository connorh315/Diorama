using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuRoom : ISchemaSerializable
    {
        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            throw new Exception("portal block not implemented");
        }
    }
}
