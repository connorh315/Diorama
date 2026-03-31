using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public struct NuTexAnimBlock
    {
        public byte ModeU;
        public byte ModeV;

        public float DU;
        public float DV;
        public float SpeedU;
        public float SpeedV;

        public byte SSNumColumns;
        public byte SSNumRows;
        public byte SSRowIndex;
        public byte SSNumImages;

        public float SSDuration;
        public byte SSOffset;

        public void Handle(SchemaSerializer schema)
        {
            schema.HandleByte(ref ModeU);
            schema.HandleByte(ref ModeV);
            schema.HandleFloat(ref DU);
            schema.HandleFloat(ref DV);
            schema.HandleFloat(ref SpeedU);
            schema.HandleFloat(ref SpeedV);
            schema.HandleByte(ref SSNumColumns);
            schema.HandleByte(ref SSNumRows);
            schema.HandleByte(ref SSRowIndex);
            schema.HandleByte(ref SSNumImages);
            schema.HandleFloat(ref SSDuration);
            schema.HandleByte(ref SSOffset);
        }
    }
}
