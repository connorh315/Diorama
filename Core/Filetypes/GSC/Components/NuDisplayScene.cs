using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuDisplayScene
    {
        public uint Version;
        
        public List<NuDefunctDisplayItem> DisplayItems;
        public List<NuClipObject> ClipObjects;
        public List<NuSpecialObject> SpecialObjects;
        public List<NuSpecialGroupNode> SpecialGroupNodes;
        public List<NuVec4> BoundsCenterAndDistSqrd;
        public List<NuVec4> BoundsExtentsAndRadius;
        public List<NuSceneInstance> SceneInstances;
        public List<ushort> SceneInstanceFixups;
        public List<uint> AnimMtls;
        public List<NuTransformMtx> TransformMtxs;
        public List<NuFaceOnDisplayItem> FaceOnDisplayItems;
        public List<short> TextureAnimListIndexs2;

        public static NuDisplayScene Read(RawFile file, NuNameTable nameTable)
        {
            Debug.Assert(file.ReadString(4) == "PSID");
            uint version = file.ReadUInt(true);
            Debug.Assert(version == 0x20 || version == 0x21, $"Unsupported DISP version: {version}");

            NuDisplayScene scene = new NuDisplayScene();
            scene.Version = version;

            scene.DisplayItems = NuSerializer.ReadVectorArray<NuDefunctDisplayItem>(file); // only for versions < 0x22
            scene.ClipObjects = NuSerializer.ReadVectorArray<NuClipObject>(file);

            scene.SpecialObjects = NuSerializer.ReadVectorArray<NuSpecialObject>(file, version);

            scene.SpecialGroupNodes = NuSerializer.ReadVectorArray<NuSpecialGroupNode>(file);

            scene.BoundsCenterAndDistSqrd = NuSerializer.ReadVectorArray<NuVec4>(file);
            scene.BoundsExtentsAndRadius = NuSerializer.ReadVectorArray<NuVec4>(file);
            scene.SceneInstances = NuSerializer.ReadVectorArray<NuSceneInstance>(file);
            scene.SceneInstanceFixups = NuSerializer.ReadVectorArray<ushort>(file); // not sure about this one - needs looking into
            Debug.Assert(scene.SceneInstanceFixups.Count == 0, "scene instance fixups != 0");
            scene.AnimMtls = NuSerializer.ReadVectorArray<uint>(file); // not sure about this one - needs looking into
            scene.TransformMtxs = NuSerializer.ReadVectorArray<NuTransformMtx>(file);
            scene.FaceOnDisplayItems = NuSerializer.ReadVectorArray<NuFaceOnDisplayItem>(file); // not sure about this one - needs looking into
            if (nameTable.Version > 0x52)
            {
                scene.TextureAnimListIndexs2 = NuSerializer.ReadLegacyVarArray<short>(file);
            }

            return scene;
        }

        public void Write(RawFile file, NuNameTable nameTable)
        {
            file.WriteString("PSID");
            file.WriteUInt(Version, true);

            NuSerializer.WriteVectorArray(file, DisplayItems);
            NuSerializer.WriteVectorArray(file, ClipObjects);
            NuSerializer.WriteVectorArray(file, SpecialObjects, Version);
            NuSerializer.WriteVectorArray(file, SpecialGroupNodes, Version);
            NuSerializer.WriteVectorArray(file, BoundsCenterAndDistSqrd);
            NuSerializer.WriteVectorArray(file, BoundsExtentsAndRadius);
            NuSerializer.WriteVectorArray(file, SceneInstances);
            NuSerializer.WriteVectorArray(file, SceneInstanceFixups);
            NuSerializer.WriteVectorArray(file, AnimMtls);
            NuSerializer.WriteVectorArray(file, TransformMtxs);
            NuSerializer.WriteVectorArray(file, FaceOnDisplayItems);
            if (nameTable.Version > 0x52)
            {
                NuSerializer.WriteLegacyVarArray(file, TextureAnimListIndexs2);
            }
        }
    }
}
