using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuCpuSkinLayer : IVectorSerializable, ISchemaSerializable
    {
        public List<NuVec> Verts;
        public List<NuCpuSkinBones> Bones;
        public List<NuUSVec> Tris;
        public List<uint> Colours;

        public short LayerId;

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

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            if (parentVersion < 4)
            {
                Debug.Assert(1 == 0, "skinlayer v < 4 not supported!");
            }
            else
            {
                schema.HandleSchemaVector(ref Verts);
                schema.HandleSchemaVector(ref Bones, parentVersion);
                schema.HandleSchemaVector(ref Tris);
                schema.HandleSerializableVector(ref Colours);
            }
        }

        public void Serialize(RawFile file, uint parentVersion)
        {
            throw new NotImplementedException();
        }
    }
}
