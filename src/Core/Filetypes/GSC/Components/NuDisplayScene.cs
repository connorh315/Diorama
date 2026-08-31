using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuDisplayScene : ISchemaSerializable
    {
        public uint Version;

        public string GscName;
        public List<NuDefunctDisplayItem> DisplayItems;
        public List<NuClipObject> ClipObjects;
        public List<uint> NumClipItems;
        public List<uint> MtlClipItems;
        public List<NuSpecialObject> SpecialObjects;
        public List<NuSpecialGroupNode> SpecialGroupNodes;
        public byte SceneSpecials;
        public List<NuVec4> BoundsCenterAndDistSqrd;
        public List<NuVec4> BoundsExtentsAndRadius;
        public List<NuSceneInstance> SceneInstances;
        public List<ushort> SceneInstanceFixups;
        public List<uint> AnimMtls;
        public List<NuTransformMtx> TransformMtxs;
        public List<NuFaceOnDisplayItem> FaceOnDisplayItems;
        public List<ushort> TextureAnimListIndexs2;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.Expect("PSID");
            schema.HandleUInt(ref Version);

            if (Version < 0x18) // no proper boundary defined anywhere for this
            {
                schema.HandleIntPascalString(ref GscName, 1);
            }

            if (Version < 0x22)
            {
                schema.HandleSchemaVector(ref DisplayItems);
                //schema.HandleSerializableVector(ref DisplayItems);
            }
            schema.HandleSchemaVector(ref ClipObjects, Version);
            if (Version < 0x18)
            {
                schema.HandleSerializableVector(ref NumClipItems);
                schema.HandleSerializableVector(ref MtlClipItems);
            }
            schema.HandleSchemaVector(ref SpecialObjects, Version);
            schema.HandleSchemaVector(ref SpecialGroupNodes, Version);
            if (Version < 0x18)
            {
                schema.HandleByte(ref SceneSpecials);
            }
            schema.HandleSchemaVector(ref BoundsCenterAndDistSqrd);
            schema.HandleSchemaVector(ref BoundsExtentsAndRadius);
            schema.HandleSchemaVector(ref SceneInstances, Version);
            schema.HandleSerializableVector(ref SceneInstanceFixups);
            schema.HandleSerializableVector(ref AnimMtls);
            if (Version < 0x1d)
            {
                schema.HandleSchemaVarArray(ref TransformMtxs);
            }
            else
            {
                schema.HandleSchemaVector(ref TransformMtxs);
            }
            schema.HandleSchemaVector(ref FaceOnDisplayItems);
            if (parentVersion > 0x52)
            {
                schema.HandleLegacyVarArray(ref TextureAnimListIndexs2);
            }
        }
    }
}
