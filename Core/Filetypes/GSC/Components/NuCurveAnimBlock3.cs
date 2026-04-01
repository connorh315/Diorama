using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuCurveAnimBlock3 : ISchemaSerializable
    {
        public int IResult;

        public short FirstFrame;
        public short NumFrames;

        public byte AnimActive;
        public byte PostAnimCycling;
        public byte PreAnimCycling;
        public byte UpdateFlag;
        public byte AttributeId;

        // Version > 4
        public byte UsesSpriteSheet;
        public byte SSFaceOn;

        // Unknown bytes
        public byte Undefined1;
        public byte Undefined2;
        public byte Undefined3;
        public byte Undefined4;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandleInt(ref IResult);

            schema.HandleShort(ref FirstFrame);
            schema.HandleShort(ref NumFrames);

            schema.HandleByte(ref AnimActive);
            schema.HandleByte(ref PostAnimCycling);
            schema.HandleByte(ref PreAnimCycling);
            schema.HandleByte(ref UpdateFlag);
            schema.HandleByte(ref AttributeId);

            // Version > 4
            schema.HandleByte(ref UsesSpriteSheet);
            schema.HandleByte(ref SSFaceOn);

            // Unknown bytes
            schema.HandleByte(ref Undefined1);
            schema.HandleByte(ref Undefined2);
            schema.HandleByte(ref Undefined3);
            schema.HandleByte(ref Undefined4);
        }
    }
}
