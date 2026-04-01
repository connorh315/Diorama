using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core
{
    public interface ISchemaSerializable
    {
        public void Handle(SchemaSerializer schema, uint parentVersion);
    }
}
