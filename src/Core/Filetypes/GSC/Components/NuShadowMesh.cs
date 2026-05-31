using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuShadowMesh : IVectorSerializable, ISchemaSerializable
    {
        public List<NuVec4> Normals;
        public List<NuVec4> Verts;

        public void Deserialize(RawFile file, uint parentVersion)
        {
            if (parentVersion > 0xe)
            {
                List<NuVec4> normals = NuSerializer.ReadVectorArray<NuVec4>(file);
                List<NuVec4> verts = NuSerializer.ReadVectorArray<NuVec4>(file);
            }
            else
            {
                List<NuVec4> normals = NuSerializer.ReadLegacyVarArray<NuVec4>(file);
                List<NuVec4> verts = NuSerializer.ReadLegacyVarArray<NuVec4>(file);
            }
        }

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            if (parentVersion < 0xe)
            {
                schema.HandleSchemaVector(ref Normals);
                schema.HandleSchemaVector(ref Verts);
            }
            else
            {
                schema.HandleSchemaVarArray(ref Normals);
                schema.HandleSchemaVarArray(ref Verts);
            }
        }

        public void Serialize(RawFile file, uint parentVersion)
        {
            throw new NotImplementedException();
        }
    }
}
