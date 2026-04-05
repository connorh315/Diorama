using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static BrickVault.Types.DATFile;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuLSVOctree : ISchemaSerializable
    {
        public uint Version;

        public Vector3 Vec1;
        public Vector3 Vec2;

        public byte MaxDepth;

        public List<NuLSVSample> Samples;
        public List<NuLSVNode> Nodes;

        public uint DlgtVersion;

        public List<NuLight> Lights;

        public byte IsNxg;
        public byte IsDx11;

        public List<ushort> LsvCompact;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.Expect("4LVI");
            schema.HandleUInt(ref Version);

            schema.HandleVector3(ref Vec1);
            schema.HandleVector3(ref Vec2);

            schema.HandleByte(ref MaxDepth);

            schema.HandleSchemaVarArray(ref Samples);
            schema.HandleSchemaVarArray(ref Nodes);

            if (Version > 2)
            {
                schema.Expect("TGLD");
                schema.HandleUInt(ref DlgtVersion);

                Debug.Assert(DlgtVersion == 0x31 || DlgtVersion == 0x33 || DlgtVersion == 0x34 || DlgtVersion == 0x36 || DlgtVersion == 0x37 || DlgtVersion == 0x39, $"dlgtversion not supported: {DlgtVersion}");

                schema.HandleSchemaVarArray(ref Lights, DlgtVersion);

                if (Version > 3)
                {
                    schema.HandleByte(ref IsNxg);
                    if (Version > 9)
                    {
                        schema.HandleByte(ref IsDx11);
                    }
                    if (Version > 4)
                    {
                        schema.HandleLegacyVarArray(ref LsvCompact);
                        Debug.Assert(LsvCompact.Count == 0, "lsv_compact not implemented!");
                    }
                }
            }
        }
    }
}
