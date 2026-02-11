using Diorama.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Filetypes.GSC.Components
{
    public class NuCharacterData : IVectorSerializable
    {
        public void Deserialize(RawFile file, uint parentVersion)
        {
            Debug.Assert(file.ReadString(4) == "LOGH");
            uint version = file.ReadUInt(true);
            Debug.Assert(version == 0x10 || version == 0x11, $"hgol version: {version:X2}");
            if (version < 0xc)
            {
                Debug.Assert(1 == 0, "unsupported HGOL version!");
            }
            else
            {
                List<NuJointData> jointData = NuSerializer.ReadVectorArray<NuJointData>(file, version);
                List<NuMtx> T = NuSerializer.ReadVectorArray<NuMtx>(file);
                List<NuMtx> inv_wt = NuSerializer.ReadVectorArray<NuMtx>(file);
                List<byte> jointIxs = NuSerializer.ReadVectorArray<byte>(file);
                List<NuPointOfInterest> pointsOfInterest = NuSerializer.ReadVectorArray<NuPointOfInterest>(file, version);
                List<byte> poiIxs = NuSerializer.ReadVectorArray<byte>(file);

                uint buffer_size = file.ReadUInt(true); // I think
                Debug.Assert(buffer_size == 0, "hgol buffer size != 0");

                List<NuLayer_SpecialFlags> layerMetaData = NuSerializer.ReadVectorArray<NuLayer_SpecialFlags>(file, version);
                List<NuLayerData> layers = NuSerializer.ReadVectorArray<NuLayerData>(file, version);

                List<NuShadowData> shadowData = NuSerializer.ReadVectorArray<NuShadowData>(file, version);

                float sphereRadius = file.ReadFloat(true);
                float sphereYOff = file.ReadFloat(true);
                Vector3 min = new Vector3(file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true));
                Vector3 max = new Vector3(file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true));
                float cylinderYOff = file.ReadFloat(true);
                float cylinderHeight = file.ReadFloat(true);
                float cylinderRadius = file.ReadFloat(true);
                float lodBoundary = file.ReadFloat(true);

                if (version < 0x10)
                {
                    List<byte> defunctTopLodRemapTable = NuSerializer.ReadVectorArray<byte>(file);
                }

                List<byte> lodRemapTable = NuSerializer.ReadVectorArray<byte>(file);

                if (version < 0x10)
                {
                    float deprecatedModelRenderScale = file.ReadFloat(true);
                }

                List<short> lodSpecialRemapTable = NuSerializer.ReadVectorArray<short>(file);

                byte krawlyLod = file.ReadByte();

                if (version > 0x10)
                {
                    uint jointNameHash = file.ReadUInt(true);
                    byte isReplacementMesh = file.ReadByte();
                }

            }
        }
    }
}
