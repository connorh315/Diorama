using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuInstAnim : ISchemaSerializable
    {
        public NuMtx Mtx = new NuMtx();

        public float TFactor;
        public float TFirst;
        public float TInterval;
        public float LocalTime;

        public ushort NumFrames;
        public uint NumTextAnims;
        public ushort TextureAnimListIdx;
        public ushort GroupNode;

        public uint Undefined1; // TODO
        public uint Undefined2;
        public uint Undefined3;

        public uint AnimIdx;
        public uint StateAnimIdx;

        public ushort InstanceIdx;
        public ushort BsobjIx;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            Mtx.Handle(schema, parentVersion);

            schema.HandleFloat(ref TFactor);
            schema.HandleFloat(ref TFirst);
            schema.HandleFloat(ref TInterval);
            schema.HandleFloat(ref LocalTime);

            if (parentVersion > 3)
            {
                schema.HandleUShort(ref NumFrames);
                schema.HandleUInt(ref NumTextAnims);
                schema.HandleUShort(ref TextureAnimListIdx);
                schema.HandleUShort(ref GroupNode);
            }

            schema.HandleUInt(ref Undefined1); // TODO
            schema.HandleUInt(ref Undefined2);
            schema.HandleUInt(ref Undefined3);

            schema.HandleUInt(ref AnimIdx);
            schema.HandleUInt(ref StateAnimIdx);

            schema.HandleUShort(ref InstanceIdx);
            schema.HandleUShort(ref BsobjIx);
        }
    }
}
