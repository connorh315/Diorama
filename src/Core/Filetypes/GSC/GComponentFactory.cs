using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC
{
    public class GComponentFactory
    {
        public static T Parse<T>(RawFile file, uint parentVersion = 0) where T : ISchemaSerializable, new()
        {
            T component = new T();

            var schema = new SchemaSerializer(file, false);
            component.Handle(schema, parentVersion);

            return component;
        }
    }
}
