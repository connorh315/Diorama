using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuShadowMesh : IVectorSerializable
    {
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
    }
}
