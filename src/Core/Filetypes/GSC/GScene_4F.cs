using Diorama.Core.Filetypes.GSC.Components;
using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC
{
    public class GScene_4F : GScene, ISchemaSerializable
    {
        public NuSceneInfo SceneInfo;

        public NuNameTable NameTable;

        public NuTextureHeaders TextureHeaders;

        public List<NuSpline> Splines;

        public List<NuVfxLocator> VfxLocators;

        public List<NuMtlOldReferencedMaterial> EmbeddedTextures;

        public byte[] UnkData;

        //public List<NuCpuSkinLod> CpuSkinLods;

        public NuCpuSkinnedblock CpuSkinnedBlock;

        public NuTextureAnim3SceneBlock TextureAnim3SceneBlock;

        public float PlaybackFPS;

        public NuAnimSceneBlock AnimSceneBlock;

        public NuBlendCharShapeBlock BlendCharShapeBlock;

        public NuOccluderBlock OccluderBlock;

        public NuIrradianceBlock IrradianceBlock;

        public NuOctreeBlock OctreeBlock;

        public List<NuCharacterData> CharacterData;

        public uint OldWiiMeshSceneBlockLinkArrayCount;

        public NuMetadataBlock Metadata;

        public uint TexHdrSceneBlock;

        public byte UseSingleLodAnim;

        public uint NumBlendShapes;

        public List<ushort> Padding;

        public List<NuSharedScene> SharedScenes;

        public Vector3 WorldBoundsCentre;
        public Vector3 WorldBoundsExtents;

        protected virtual void ReadCpuSkinned()
        {
            Debug.Assert(file.ReadString(4) == "SUPC");
            uint version = file.ReadUInt(true);
            Debug.Assert(version == 4);

            if (version < 3)
            {
                Debug.Assert(false, "CPUS section not supported");
            }
            else if (version == 3)
            {

            }
            else
            {
                //CpuSkinLods = NuSerializer.ReadVectorArray<NuCpuSkinLod>(file, version);
            }
        }

        public byte WasGeneratedFromLEDImport;

        protected override void Parse(GSerializationContext ctx)
        {
            SchemaSerializer schema = new SchemaSerializer(file, false);
            schema.SetContext(ctx);

            SceneInfo = NuSceneInfo.Read(file, NU20Version);

            if (NU20Version > 0x56)
            {
                WasGeneratedFromLEDImport = file.ReadByte();
            }

            NameTable = NuNameTable.Read(file);

            schema.HandleOptional(ref TextureHeaders, NU20Version);
            if (TextureHeaders != null)
            {
                ctx.AddReference(TextureHeaders);
            }

            schema.HandleSchemaVector(ref Splines, NU20Version);

            VfxLocators = NuSerializer.ReadVectorArray<NuVfxLocator>(file);

            Debug.Assert(file.ReadUInt(true) == 1); // possibly number of MESHes?

            MeshSceneBlock = NuMeshSceneBlock.Parse(file, ctx);

            Debug.Assert(file.ReadUInt(true) == 0);
            Debug.Assert(file.ReadUInt(true) == 1);

            var matBlock = new NuMaterialDataBlock();
            matBlock.Handle(schema, NU20Version);
            MaterialBlock = matBlock;
            //MaterialBlock = GComponentFactory.Parse<NuMaterialDataBlock>(file, NU20Version);

            //Materials = NuMaterialData.Read(file);

            //EmbeddedTextures = NuSerializer.ReadVectorArray<NuMtlOldReferencedMaterial>(file);

            //UnkData = file.ReadArray(9);
            
            uint lightmapCount = file.ReadUInt(true);
            Debug.Assert(lightmapCount == 1);
            LightmapDataBlock = NuLightmapDataBlock.Parse(file);

            uint cpusCount = file.ReadUInt(true);
            Debug.Assert(cpusCount == 1);
            CpuSkinnedBlock = GComponentFactory.Parse<NuCpuSkinnedblock>(file, NU20Version);

            DisplayScene = NuDisplayScene.Read(file, NameTable);

            TextureAnim3SceneBlock = GComponentFactory.Parse<NuTextureAnim3SceneBlock>(file);

            PlaybackFPS = file.ReadFloat(true);

            Debug.Assert(file.ReadUInt(true) == 1);
            AnimSceneBlock = NuAnimSceneBlock.Parse(file);

            Debug.Assert(file.ReadUInt(true) == 0); // possibly portal instances? (SNIP)

            BlendCharShapeBlock = NuBlendCharShapeBlock.Parse(file);

            OccluderBlock = NuOccluderBlock.Parse(file);

            if (NU20Version < 0x4f)
            {
                NuIrradianceBlock block = GComponentFactory.Parse<NuIrradianceBlock>(file);
            }

            //OctreeBlock = NuOctreeBlock.Parse(file);

            OctreeBlock = GComponentFactory.Parse<NuOctreeBlock>(file, NU20Version);

            if (NameTable.Version < 0x14)
            {
                CharacterData = NuSerializer.ReadLegacyVarArray<NuCharacterData>(file);
            }
            else
            {
                CharacterData = NuSerializer.ReadVectorArray<NuCharacterData>(file);
            }

            OldWiiMeshSceneBlockLinkArrayCount = file.ReadUInt(true);

            Metadata = NuMetadataBlock.Parse(file);

            if (NameTable.Version < 0x51)
            {
                TexHdrSceneBlock = file.ReadUInt(true);
                Debug.Assert(TexHdrSceneBlock == 0);
            }

            UseSingleLodAnim = file.ReadByte();

            NumBlendShapes = file.ReadUInt(true);

            Padding = NuSerializer.ReadVectorArray<ushort>(file); // pad data

            if (NameTable.Version > 0x46)
            {
                SharedScenes = new();

                NuSerializer.ReadLegacyVarArray<NuSharedScene>(file);

                //uint sharedScenesSize = file.ReadUInt(true);
                //if (sharedScenesSize != 0)
                //{
                //    for (int i = 0; i < sharedScenesSize; i++)
                //    {
                //        string resourceId = file.ReadPascalString();
                //        string objectId = file.ReadPascalString();
                //        SharedScenes.Add((resourceId, objectId));
                //    }
                //}
            }

            if (NameTable.Version > 0x54)
            {
                WorldBoundsCentre = file.ReadVector3(true);
                WorldBoundsExtents = file.ReadVector3(true);
            }
        }

        internal override void WriteNu20(RawFile file, GSerializationContext ctx)
        {
            SchemaSerializer schema = new SchemaSerializer(file, true);

            SceneInfo.Write(file, NU20Version);

            if (NU20Version > 0x56)
            {
                file.WriteByte(WasGeneratedFromLEDImport);
            }

            NameTable.Write(file);

            schema.HandleOptional(ref TextureHeaders, NU20Version);

            if (TextureHeaders != null)
            {
                ctx.AddReference(TextureHeaders);
            }

            schema.HandleSchemaVector(ref Splines, NU20Version);
            //NuSerializer.WriteVectorArray(file, Splines, NU20Version);
            //if (Splines.Count != 0)
            //{
            //    throw new NotSupportedException("cannot write splines");
            //}

            NuSerializer.WriteVectorArray(file, VfxLocators);
            if (VfxLocators.Count != 0)
            {
                throw new NotSupportedException("cannot write vfx locators");
            }

            file.WriteInt(1, true);
            MeshSceneBlock.Write(file, ctx);

            file.WriteInt(0, true);
            file.WriteInt(1, true);

            MaterialBlock.Handle(schema, NU20Version);

            //file.WriteString("LTMU");
            //file.WriteUInt(Materials[0].Version, true);
            //file.WriteInt(Materials.Length, true);

            //foreach (var mat in Materials)
            //{
            //    mat.Write(file);
            //}

            //NuSerializer.WriteVectorArray(file, EmbeddedTextures);

            //file.WriteArray(UnkData);

            file.WriteInt(1, true);
            LightmapDataBlock.Write(file);

            file.WriteInt(1, true); // cpu skinned marker
            file.WriteString("SUPC");
            file.WriteInt(4, true); // TODO: pull this properly
            //NuSerializer.WriteVectorArray(file, CpuSkinLods);
            //if (CpuSkinLods.Count != 0)
            //{
            //    throw new NotSupportedException("cannot write cpu skin lods");
            //}

            DisplayScene.Write(file, NameTable);

            TextureAnim3SceneBlock.Handle(schema, NU20Version);

            file.WriteFloat(PlaybackFPS, true);

            file.WriteInt(1, true);
            AnimSceneBlock.Write(file);

            file.WriteInt(0, true); // portal instances?

            BlendCharShapeBlock.Write(file);

            OccluderBlock.Write(file);

            OctreeBlock.Handle(schema, NU20Version);

            if (NameTable.Version < 0x14)
            {
                NuSerializer.WriteLegacyVarArray(file, CharacterData);
            }
            else
            {
                NuSerializer.WriteVectorArray(file, CharacterData);
            }

            file.WriteUInt(OldWiiMeshSceneBlockLinkArrayCount, true);

            Metadata.Write(file);

            if (NameTable.Version < 0x51)
            {
                file.WriteUInt(TexHdrSceneBlock, true);
            }

            file.WriteByte(UseSingleLodAnim);

            file.WriteUInt(NumBlendShapes, true);

            NuSerializer.WriteVectorArray(file, Padding);

            if (NameTable.Version > 0x46)
            {
                file.WriteInt(SharedScenes.Count, true);

                for (int i = 0; i < SharedScenes.Count; i++)
                {
                    //file.WritePascalString(SharedScenes[i].Item1);
                    //file.WritePascalString(SharedScenes[i].Item2);
                }
            }

            if (NameTable.Version > 0x54)
            {
                file.WriteVector3(WorldBoundsCentre, true);
                file.WriteVector3(WorldBoundsExtents, true);
            }
        }

        public override void Handle(SchemaSerializer schema, uint parentVersion)
        {
            int unknownSection = 0;

            GSerializationContext ctx = new GSerializationContext();
            schema.SetContext(ctx);

            schema.Handle(ref SceneInfo);

            if (NU20Version > 0x56)
            {
                schema.HandleByte(ref WasGeneratedFromLEDImport);
            }

            schema.Handle(ref NameTable);

            schema.HandleOptional(ref TextureHeaders);
            if (TextureHeaders != null)
            {
                ctx.AddReference(TextureHeaders);
            }

            schema.HandleSchemaVector(ref Splines, NU20Version);

            schema.HandleSchemaVector(ref VfxLocators);

            schema.HandleOptional(ref MeshSceneBlock);

            schema.HandleInt(ref unknownSection);
            Debug.Assert(unknownSection == 0);

            schema.HandleOptional(ref MaterialBlock);

            schema.HandleOptional(ref LightmapDataBlock);

            schema.HandleOptional(ref CpuSkinnedBlock, NU20Version);

            schema.Handle(ref DisplayScene, parentVersion);

            schema.Handle(ref TextureAnim3SceneBlock);

            schema.HandleFloat(ref PlaybackFPS);

            schema.HandleOptional(ref AnimSceneBlock);

            schema.HandleInt(ref unknownSection);
            Debug.Assert(unknownSection == 0);

            schema.Handle(ref BlendCharShapeBlock);

            schema.Handle(ref OccluderBlock);

            if (NU20Version < 0x4f)
            {
                schema.Handle(ref IrradianceBlock);
            }

            schema.Handle(ref OctreeBlock, NU20Version);

            if (NameTable.Version < 0x14)
            {
                schema.HandleSerializableLegacyVarArray(ref CharacterData);
            }
            else
            {
                schema.HandleSerializableVector(ref CharacterData);
            }

            schema.HandleUInt(ref OldWiiMeshSceneBlockLinkArrayCount);

            schema.Handle(ref Metadata);

            if (NameTable.Version < 0x51)
            {
                schema.HandleInt(ref unknownSection);
                Debug.Assert(unknownSection == 0, "texhdrsceneblock exists");
            }

            schema.HandleByte(ref UseSingleLodAnim);

            schema.HandleUInt(ref NumBlendShapes);

            schema.HandleSerializableVector(ref Padding);

            if (NameTable.Version > 0x46)
            {
                schema.HandleSchemaVarArray(ref SharedScenes);
            }

            if (NameTable.Version > 0x54)
            {
                schema.HandleVector3(ref WorldBoundsCentre);
                schema.HandleVector3(ref WorldBoundsExtents);

            }
        }
    }
}
