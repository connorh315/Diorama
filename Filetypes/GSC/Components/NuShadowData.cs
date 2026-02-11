using Diorama.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Filetypes.GSC.Components
{
    public class NuShadowData : IVectorSerializable
    {
        public void Deserialize(RawFile file, uint parentVersion)
        {
            if (parentVersion < 0xf)
            {
                List<NuEllipsoid> ellipsoids = NuSerializer.ReadLegacyVarArray<NuEllipsoid>(file);
                List<NuCylinder> cylinders = NuSerializer.ReadLegacyVarArray<NuCylinder>(file);
                List<NuShadowMesh> shadowMeshes = NuSerializer.ReadLegacyVarArray<NuShadowMesh>(file, parentVersion);
            }
            else
            {
                List<NuEllipsoid> ellipsoids = NuSerializer.ReadVectorArray<NuEllipsoid>(file);
                List<NuCylinder> cylinders = NuSerializer.ReadVectorArray<NuCylinder>(file);
                List<NuShadowMesh> shadowMeshes = NuSerializer.ReadVectorArray<NuShadowMesh>(file, parentVersion);
            }
            byte joint = file.ReadByte();
        }
    }
}
