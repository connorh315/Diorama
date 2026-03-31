using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core
{
    public class SchemaSerializer
    {
        public RawFile File { get; private set; }
        public bool Writing { get; private set; }
        public SchemaSerializer(RawFile file, bool writing)
        {
            File = file;
            Writing = writing;
        }

        public void HandleFloat(ref float v)
        {
            if (Writing)
            {
                File.WriteFloat(v, true);
            }
            else
            {
                v = File.ReadFloat(true);
            }
        }

        public void HandleUInt(ref uint v)
        {
            if (Writing)
            {
                File.WriteUInt(v, true);
            }
            else
            {
                v = File.ReadUInt(true);
            }
        }

        public void HandleInt(ref int v)
        {
            if (Writing)
            {
                File.WriteInt(v, true);
            }
            else
            {
                v = File.ReadInt(true);
            }
        }

        public void HandleShort(ref short v)
        {
            if (Writing)
            {
                File.WriteShort(v, true);
            }
            else
            {
                v = File.ReadShort(true);
            }
        }

        public void HandleByte(ref byte v)
        {
            if (Writing)
            {
                File.WriteByte(v);
            }
            else
            {
                v = File.ReadByte();
            }
        }
    }
}
