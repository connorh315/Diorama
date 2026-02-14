using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuCpuSkinLayer : IVectorSerializable
    {
        public void Deserialize(RawFile file, uint parentVersion)
        {
            if (parentVersion < 4)
            {

            }
            else
            {
                List<NuVec> verts = NuSerializer.ReadVectorArray<NuVec>(file);
                List<NuCpuSkinBones> bones = NuSerializer.ReadVectorArray<NuCpuSkinBones>(file, parentVersion);
                List<NuUSVec> tris = NuSerializer.ReadVectorArray<NuUSVec>(file);
                List<uint> colours = NuSerializer.ReadVectorArray<uint>(file);
            }

            short layerId = file.ReadShort(true);
        }
    }
}
