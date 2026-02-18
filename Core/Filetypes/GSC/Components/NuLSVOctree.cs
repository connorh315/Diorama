using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuLSVOctree : IVectorSerializable
    {
        public void Deserialize(RawFile file, uint parentVersion)
        {
            Debug.Assert(file.ReadString(4) == "4LVI");
            uint version_4 = file.ReadUInt(true); // 0xa

            Vector3 vec1 = new Vector3(file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true));
            Vector3 vec2 = new Vector3(file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true));

            byte maxDepth = file.ReadByte();
            List<NuLSVSample> samples = NuSerializer.ReadLegacyVarArray<NuLSVSample>(file);
            List<NuLSVNode> nodes = NuSerializer.ReadLegacyVarArray<NuLSVNode>(file);

            if (version_4 > 2)
            {
                Debug.Assert(file.ReadString(4) == "TGLD");
                uint dlgtVersion = file.ReadUInt(true);
                Debug.Assert(dlgtVersion == 0x34 || dlgtVersion == 0x36 || dlgtVersion == 0x37 || dlgtVersion == 0x39, $"dlgtversion not supported: {dlgtVersion}");
                List<NuLight> lights = NuSerializer.ReadLegacyVarArray<NuLight>(file, dlgtVersion);
                if (version_4 > 3)
                {
                    byte is_nxg = file.ReadByte();
                    if (version_4 > 9)
                    {
                        byte is_dx11 = file.ReadByte();
                    }
                    if (version_4 > 4)
                    {
                        List<ushort> lsv_compact = NuSerializer.ReadLegacyVarArray<ushort>(file);
                        Debug.Assert(lsv_compact.Count == 0, "lsv_compact not implemented!");

                    }
                }
            }
        }
    }
}
