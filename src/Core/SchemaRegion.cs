using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core
{
    public class SchemaRegion : IDisposable
    {
        private int Size = 0;
        private long StartPosition;
        private SchemaSerializer Schema;

        public SchemaRegion(SchemaSerializer schema)
        {
            Schema = schema;
            schema.HandleInt(ref Size);
            StartPosition = schema.File.Position;
        }

        public void Dispose()
        {
            RawFile file = Schema.File;
            long endPosition = Schema.File.Position;
            int length = (int)(endPosition - StartPosition);
            if (Schema.Writing)
            {
                file.Seek(StartPosition - 4, SeekOrigin.Begin);
                file.WriteInt(length, true);
                file.Seek(endPosition, SeekOrigin.Begin);
            }
            else
            {
                Debug.Assert(length == Size, "Did not read all of block!");
            }
        }
    }
}
