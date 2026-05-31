using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuShadowData : IVectorSerializable, ISchemaSerializable
    {
        public List<NuEllipsoid> Ellipsoids;
        public List<NuCylinder> Cylinders;
        public List<NuShadowMesh> ShadowMeshes;

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

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            if (parentVersion < 0xf)
            {
                schema.HandleSchemaVarArray(ref Ellipsoids);
                schema.HandleSchemaVarArray(ref Cylinders);
                schema.HandleSchemaVarArray(ref ShadowMeshes, parentVersion);
            }
            else
            {
                schema.HandleSchemaVector(ref Ellipsoids);
                schema.HandleSchemaVector(ref Cylinders);
                schema.HandleSchemaVector(ref ShadowMeshes, parentVersion);
            }
        }

        public void Serialize(RawFile file, uint parentVersion)
        {
            throw new NotImplementedException();
        }
    }
}
