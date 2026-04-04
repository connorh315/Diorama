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
    public class GScene_4F : GScene
    {
        public NuSceneInfo SceneInfo { get; set; }

        public NuNameTable NameTable { get; set; }

        public NuTextureHeaders TextureHeaders { get; set; }

        public List<NuSpline> Splines;

        public List<NuVfxLocator> VfxLocators { get; set; }

        public List<NuMtlOldReferencedMaterial> EmbeddedTextures { get; set; }

        public byte[] UnkData { get; set; }

        public List<NuCpuSkinLod> CpuSkinLods { get; set; }

        public NuTextureAnim3SceneBlock TextureAnim3SceneBlock { get; set; }

        public float PlaybackFPS;

        public NuAnimSceneBlock AnimSceneBlock { get; set; }

        public NuBlendCharShapeBlock BlendCharShapeBlock { get; set; }

        public NuOccluderBlock OccluderBlock { get; set; }

        public NuOctreeBlock OctreeBlock { get; set; }

        public List<NuCharacterData> CharacterData { get; set; }

        public uint OldWiiMeshSceneBlockLinkArrayCount;

        public NuMetadataBlock Metadata { get; set; }

        public uint TexHdrSceneBlock { get; set; }

        public byte UseSingleLodAnim { get; set; }

        public uint NumBlendShapes { get; set; }

        public List<ushort> Padding { get; set; }

        public List<(string, string)> SharedScenes { get; set; }

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
                CpuSkinLods = NuSerializer.ReadVectorArray<NuCpuSkinLod>(file, version);
            }
        }

        public byte WasGeneratedFromLEDImport;

        protected override void Parse(GSerializationContext ctx)
        {
            SchemaSerializer schema = new SchemaSerializer(file, false);

            SceneInfo = NuSceneInfo.Read(file, NU20Version);

            if (NU20Version > 0x56)
            {
                WasGeneratedFromLEDImport = file.ReadByte();
            }

            NameTable = NuNameTable.Read(file);

            uint hasTextureHeaderComponent = file.ReadUInt(true);
            if (hasTextureHeaderComponent != 0)
            {
                TextureHeaders = NuTextureHeaders.Read(file);
                ctx.AddReference(TextureHeaders);
                Debug.Assert(hasTextureHeaderComponent == 1);
            }

            schema.HandleSchemaVector(ref Splines, NU20Version);

            VfxLocators = NuSerializer.ReadVectorArray<NuVfxLocator>(file);

            Debug.Assert(file.ReadUInt(true) == 1); // possibly number of MESHes?

            MeshSceneBlock = NuMeshSceneBlock.Parse(file, ctx);

            Debug.Assert(file.ReadUInt(true) == 0);
            Debug.Assert(file.ReadUInt(true) == 1);

            var matBlock = new NuMaterialDataBlock();
            matBlock.Handle(schema, NU20Version, ctx);
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
            ReadCpuSkinned();

            DisplayScene = NuDisplayScene.Read(file, NameTable);

            TextureAnim3SceneBlock = GComponentFactory.Parse<NuTextureAnim3SceneBlock>(file);

            PlaybackFPS = file.ReadFloat(true);

            Debug.Assert(file.ReadUInt(true) == 1);
            AnimSceneBlock = NuAnimSceneBlock.Parse(file);

            Debug.Assert(file.ReadUInt(true) == 0); // possibly portal instances? (SNIP)

            BlendCharShapeBlock = NuBlendCharShapeBlock.Parse(file);

            OccluderBlock = NuOccluderBlock.Parse(file);

            //OctreeBlock = NuOctreeBlock.Parse(file);

            OctreeBlock = GComponentFactory.Parse<NuOctreeBlock>(file);

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

                uint sharedScenesSize = file.ReadUInt(true);
                if (sharedScenesSize != 0)
                {
                    for (int i = 0; i < sharedScenesSize; i++)
                    {
                        string resourceId = file.ReadPascalString();
                        string objectId = file.ReadPascalString();
                        SharedScenes.Add((resourceId, objectId));
                    }
                }
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

            if (TextureHeaders != null)
            {
                throw new NotSupportedException("cannot write texture headers");
            }
            else
            {
                file.WriteInt(0);
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

            MaterialBlock.Handle(schema, NU20Version, ctx);

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
            NuSerializer.WriteVectorArray(file, CpuSkinLods);
            if (CpuSkinLods.Count != 0)
            {
                throw new NotSupportedException("cannot write cpu skin lods");
            }

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
                    file.WritePascalString(SharedScenes[i].Item1);
                    file.WritePascalString(SharedScenes[i].Item2);
                }
            }

            if (NameTable.Version > 0x54)
            {
                file.WriteVector3(WorldBoundsCentre, true);
                file.WriteVector3(WorldBoundsExtents, true);
            }
        }
    }
}
