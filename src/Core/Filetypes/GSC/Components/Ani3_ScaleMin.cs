using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class Ani3_ScaleMin : ISchemaSerializable
    {
        public float Scale;
        public float Min;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandleFloat(ref Scale);
            schema.HandleFloat(ref Min);
        }
    }
}
