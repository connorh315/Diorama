using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.Marshalling;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public abstract class NuMaterialData : ISchemaSerializable
    {
        public uint ExtraStarter;

        public uint Version;

        public int Diffuse0Index;
        public int Diffuse1Index;
        public int Diffuse2Index;
        public int Diffuse3Index;

        public int Specular0Index;
        public int Specular1Index;

        public int Normal0Index;
        public int Normal1Index;

        public int EnvMap;

        public int VTFH;
        public int VTFN;
        public int DiffEnv;
        public int TexAnimMapTID;
        public int TexAnimCurvesTID;

        public int Normal2;
        public int Specular2;

        public int Normal3;
        public int Specular3;

        public int Detail0;
        public int Detail1;
        public int Detail2;
        public int Detail3;

        public float DetailRepeat0;
        public float DetailRepeat1;
        public float DetailRepeat2;
        public float DetailRepeat3;

        public NuTexAuxEntry[] TexAuxEntries;

        public int TexAnimData1;
        public int TexAnimData2;
        public int TexAnimData3;
        public int TexAnimData4;

        public NuTexAnimBlock[] TexAnimBlocks = new NuTexAnimBlock[4];

        public byte VertexFlags_VertexControlledTint;

        public int Colour1;
        public int Colour2;
        public int Colour3;
        public int Colour4;

        public byte BitangentFlip;

        public float KNormal0;
        public float KNormal1;
        public float KNormal2;
        public float KNormal3;

        public float KParallax;
        public float KParallaxBias;

        public int Colour5;
        public int Colour6;
        public int Colour7;
        public int Colour8;
        public int Colour9;
        public int Colour10;
        public int Colour11;
        public int Colour12;

        public float KRefractiveIndex;
        public float KRefractiveThicknessFactor;
        public float KGlow;

        public int Colour13;

        public float KBaseReflectivity;
        public float KBaseSpecularCosPower;
        public float KCustomEnvMapStrength;

        public float KEnvLighting; // version < 0xe7

        public float KEnvAlphaHDR;
        public float KAlphaFresnelConst;
        public float KAlphaFresnelPower;
        public float KVTFHeight;
        public float KVTFNormal;
        public int KVTFOffset;

        public float KVTFDirection;
        public float KVTFDirection1;
        public float KVTFDirection2;
        public float KUseVTFDirection;

        public int Vtfh2;

        public int Colour14;
        public int Colour15;

        public int Colour16; // Version < 0xdb
        public int Colour17; // Version < 0xdb

        public float KCarPaintViewFactor;
        public float KCarPaintLightFactor;
        public float KBaseRoughness;

        public float DefunctKBRDFAnotherSetting;   // Version < 0xe0
        public float DefunctKFractalFrequency;     // Version < 0xe0
        public float DefunctKFractalDiffuse;       // Version < 0xe0
        public float DefunctKFractalSpecular;      // Version < 0xe0
        public float DefunctKFractalLacunarity;    // Version < 0xe0
        public float DefunctKFractalGain;          // Version < 0xe0
        public float DefunctKFractalHeight;        // Version < 0xe0

        public float KEnvLightIntensity;
        public float KEnvLightSpecular;
        public float KEnvFresnel;
        public float KEnvFresnelPower;

        public float KSpecularBump; // Version < 0xe6
        public float KSkinSpread;   // Version < 0xe6

        public float KBaseSubstance;

        public byte KDepthBias;

        public float KDepthBiasScale;
        public float KShadowBias;

        public float KRoomWidth;   // Version > 0xde
        public float KRoomHeight;  // Version > 0xde
        public float KRoomDepth;   // Version > 0xde

        public float KWiiMaxAlphaBias;

        public int Colour18;
        public int Colour19;

        public short KTPageID;

        public float KStiffness;

        public float PerLayerUVScale1; // Version >= 0xd1
        public float PerLayerUVScale2; // Version >= 0xd1
        public float PerLayerUVScale3; // Version >= 0xd1
        public float PerLayerUVScale4; // Version >= 0xd1

        public byte KLightmapTranslucency; // Version >= 0xd1
        public byte KLightmapEmission;     // Version >= 0xd1

        public byte BForceDefaultCubeMap; // Version > 0xe3

        public float[] ShaderGraphParams = new float[8];

        public List<NuVec4> ShaderGraphParamsVector;

        public uint shader_Version;
        public short legoVersion;
        public uint shaderType;
        public uint lightingModel;
        public uint substanceMode;
        public uint roughnessMode;
        public uint fresnelAlphaMode;
        public uint blendMode;
        public uint alphaTest;
        public uint alphaFadeSource;
        public uint surfaceMapMethod;
        public uint surfaceMapFormat0;
        public uint surfaceMapFormat1;
        public uint surfaceMapFormat2;
        public uint surfaceMapFormat3;
        public uint surfaceMapFormatVTFN;
        public uint occlusion;
        public uint refraction;
        public uint reflection;
        public uint baseDiffuseUsage;
        public uint layerBlendDiffuse;
        public uint layerBlendDiffuse1;
        public uint layerBlendDiffuse2;
        public uint usesDiffuseLayerColour;
        public uint usesDiffuseLayerColour1;
        public uint usesDiffuseLayerColour2;
        public uint usesDiffuseLayerColour3;

        public uint layerBlendSpecular0;
        public uint layerBlendSpecular1;
        public uint layerBlendSpecular2;
        public uint dummy;
        public uint layerBlendNormal0;
        public uint layerBlendNormal1;
        public uint layerBlendNormal2;
        public uint dummyX;
        public uint numUVSets;
        public int LightmapUVSet;
        public uint motionBlurVertexType;
        public uint motionBlurPixelType;
        public byte motionBlurSamples;
        public byte numBones;

        public NuMtlUVBlock[] uvBlocks;
        public byte old_bitangentFlip;
        public byte materialFlags_tangentSwap;
        public byte materialFlags_water;
        public byte materialFlags_parallaxBlendFix;
        public byte old_nextgenshine;
        public byte materialFlags_glow;
        public byte materialFlags_carpaint;
        public byte old_fractalbump;
        public byte old_fractalbump2;
        public byte materialFlags_fog;
        public byte materialFlags_unlitNonSRGB;
        public byte materialFlags_hdralpha_diffuse;
        public byte materialFlags_hdralpha_envmap;
        public byte materialFlags_derivHeightMap;
        public byte materialFlags_smoothSpec;
        public byte materialFlags_zeusCompatMode;
        public byte materialFlags_disable_varying_specular;
        public byte materialFlags_disable_fresnel;
        public byte materialFlags_two_sided_lighting;
        public byte materialFlags_smoothlightmap;
        public byte materialFlags_rimlight;
        public byte materialFlags_ignore_exposure;
        public byte materialFlags_baked_specular;
        public byte materialFlags_semi_lit;
        public byte materialFlags_refractionNearFix;
        public byte materialFlags_metallic_specular;
        public byte materialFlags_dontreceiveshadow;
        public byte materialFlags_lateshader;
        public byte materialFlags_diffreflmaps;
        public byte materialFlags_per_layer_uvscale;
        public byte materialFlags_tintable;
        public byte materialFlags_generateCubeMap;
        public byte materialFlags_outputToonShaderData;
        public byte materialFlags_disablePerPixelFade;
        public byte materialFlags_cel_shading;
        public byte miscFlags_conditional_cel_shading;
        public byte materialFlags_receiveShadowDespiteCelShading;
        public byte miscFlags_useRoomProjection;
        public byte miscFlags_useCustomPixelClipPlane;
        public byte miscFlags_layer2Refraction;
        public byte miscFlags_layer3Refraction;
        public byte miscFlags_layer4Refraction;
        public byte miscFlags_layer2DX11Only;
        public byte miscFlags_layer3DX11Only;
        public byte miscFlags_layer4DX11Only;
        public byte miscFlags_allLayerVertAlbedo;
        public byte vertexFlags_skinned;
        public byte vertexFlags_fastBlend;
        public byte vertexFlags_blendShape;
        public byte vertexFlags_doPerspDivInVS;
        public byte vertexFlags_numAlphaLayers;
        public byte vertexFlags_use2DW;
        public byte vertexFlags_untransformed;
        public byte vertexFlags_effectAmplitude;
        public byte vertexFlags_ignoreVertexOpacity;
        public byte vertexFlags_unused1;
        public byte vertexFlags_instancedLightmapping;
        public byte vertexFlags_positionAccuracy;
        public byte vertexFlags_uvAccuracy;
        public byte vertexFlags_tangent2;
        public byte vertexFlags_ZBias;
        public byte vertexFlags_layer1VertAlbedo;
        public byte vertexFlags_layer2VertAlbedo;
        public byte vertexFlags_layer3VertAlbedo;
        public byte vertexFlags_disableSeparatePositionStream;
        public byte vertexFlags_legoTerrain;
        public byte vertexFlags_legoTerrainMeshType;
        public byte vertexFlags_largeWorldAwareCamera;
        public byte vertexFlags_wind;
        public byte vertexFlags_forceColourVertexStream;
        public byte vertexFlags_vertexRoughnessMod;
        public long vertexFlags_unused2;
        public byte miscFlags_greyAlbedo;
        public byte miscFlags_motionBlur;
        public byte miscFlags_UVAnimation;
        public byte miscFlags_canAlphaBlend;
        public byte miscFlags_defunctOpaque;
        public byte miscFlags_isDecal;
        public byte miscFlags_creaseMeshMaterial;
        public byte miscFlags_ttAnimationMode;
        public byte miscFlags_culled;
        public byte miscFlags_isDeferredDecal;
        public byte miscFlags_isPBRSourced;
        public byte miscFlags_requiresDiffuseAlphaMultiply;
        public byte miscFlags_isTPaged;
        public byte miscFlags_disableDynamicLighting;
        public byte miscFlags_useLayers234OnWii;
        public byte miscFlags_useWiiTintColours;
        public byte miscFlags_sRGBSupport;
        public byte miscFlags_useNormalEncodingTexture;
        public byte miscFlags_refractionIgnoreVertexNormal;
        public byte miscFlags_shadedGlow;
        public byte miscFlags_project_to_far_plane;
        public byte miscFlags_sortAfterPostEffects;
        public byte output_colourRT;
        public byte output_normalRT;
        public byte output_albedoRT;
        public byte output_depthAsColourRT;
        public uint displayMode;
        public uint shaderVersion;
        public uint gpuVendor;
        public uint colourSpace;
        public uint bakedLighting;
        public int discreteLightType;
        public int discreteLightShadingModel;
        public byte discreteLightSoftShadows;

        public NuMtlDiscreteLight[] DiscreteLight2 = new NuMtlDiscreteLight[4];

        public int sceneZAccess;
        public int shadowZAccess;
        public int pcfMethod;
        public int rainSplashSurfaceType;

        public byte DefunctOldAlpha;
        public byte DefunctOldAtst;

        public byte Afail;
        public byte Aref;
        public byte Cull;
        public byte Zmode;
        public byte StencilMode;
        public byte Noprepass;
        public byte Filter;
        public byte OldUtc;
        public byte OldVtc;
        public byte Colour;
        public byte OldFillAttrib;

        public byte DefunctOnly2D;
        public byte DefunctStencilShadows;

        public byte Castshadow;
        public byte OldAutoStencil;
        public byte ColourWriteMask;

        public byte AlwaysUpdateRefraction;
        public byte SortLast;
        public byte ExternalFixupTarget;

        public byte SortFirst;
        public byte DisableEdgeOutlines;

        public byte PreserveColoursDuringMainFilter;

        public byte AlphaTestMode;

        public uint Fx1;
        public uint Fx2;
        public uint Fx3;
        public uint Fx4;

        public int OldTid;

        public byte Fxid;
        public byte SpecialId;

        public short ShortPril16bit;

        public uint FirstVariantIdx;
        public uint NextVariantIdx;

        public byte DefunctIsCreasedMeshMaterial;
        public byte HasVariants;

        public byte LegoStudMaterial;

        public byte MaskShadows;
        public byte SortAfterRefraction;
        public byte SkipValidation;

        public byte SpecialDepthSorting;
        public byte ForceAlphaLightingSupport;

        public byte NoAutoScreenDoor;
        public byte CompileLiveCubemapGenShader;
        public byte CompileToonShader;

        public byte ShadowImpostor;
        public byte ShadowFromFrontFaces;
        public byte DoUntexturedTPage;
        public byte ForceTPageRemap;
        public byte ForceTPageSurfType;
        public byte ForceTPageAlphaFade;

        public uint DefaultRenderStage;

        //public static NuMaterialData[] Read(RawFile file)
        //{
        //    Debug.Assert(file.ReadString(4) == "LTMU");
        //    uint version = file.ReadUInt(true);
        //    uint count = file.ReadUInt(true);
        //    NuMaterialData[] materials = new NuMaterialData[count];

            
        //    if (version > 0x100) // Just a guess
        //    {
        //        int correction = file.ReadInt(true);
        //        Debug.Assert(correction == 1);
        //    }

        //    for (int i = 0; i < count; i++)
        //    {
        //        NuMaterialData materialData;
        //        switch (version)
        //        {
        //            case 0xd5:
        //            case 0xd6:
        //            case 0xd7:
        //            case 0xd8:
        //            case 0xd9:
        //            case 0xda:
        //            case 0xdb:
        //            case 0xdc:
        //            case 0xdd:
        //            case 0xde:
        //            case 0xdf:
        //            case 0xe0:
        //            case 0xe1:
        //            case 0xe2:
        //            case 0xe4:
        //            case 0xe5:
        //            case 0xe8:
        //            case 0xea:
        //            case 0xeb:
        //            case 0xec:
        //            case 0xef:
        //            case 0xf0:
        //            case 0xf1:
        //            case 0xf2:
        //            case 268:
        //                materialData = new NuMaterialData_E0();
        //                break;
        //            default:
        //                throw new Exception($"Unsupported UMTL Version: {version}");
        //        }

        //        materialData.Version = version;
        //        materialData.Parse(file);

        //        materials[i] = materialData;
        //    }

        //    return materials;
        //}

        public abstract void Handle(SchemaSerializer schema, uint parentVersion);
    }

    public class NuMaterialData_E0 : NuMaterialData
    {
        public string MaterialName;
        public uint Flags;

        public byte[] DummyHashArray;

        public VertexList VertexLayout;
        
        public uint RimLightBlendMode;
        private uint shaderFxCodeHash;
        private byte materialFlags_shaderGraphMtl;
        private byte materialFlags_IsScratchedLego;
        private byte dummy_isMayaShader;
        private byte output_tangentRT;
        private byte vertexFlags_gameFiveBitPacking;
        private byte vertexFlags_usesInstancing;
        private byte output_emissionRT;
        private byte materialFlags_depthOnly;

        public byte[] ShaderFxCode;
        private byte isReferencedMaterial;
        private string referencedMaterialName;
        private string referencedGscName;
        private byte AlphaRespondToLights;
        private byte AlphaRespondToProbes;

        public List<NuTexGenHdr> VertexFixupData;
        public List<NuTexGenHdr> PixelFixupData;

        public List<NuShaderUserParamInfo> VertexShaderConsts;
        public List<NuShaderUserParamInfo> PixelShaderConsts;
        private int packedFloatCountVertex;
        private int packedFloatCountPixel;
        public List<NuShaderUserParamInfo> VertexShaderInstancedConsts;
        public List<NuShaderUserParamInfo> PixelShaderInstancedConsts;
        private int packedInstancedFloatCountVertex;
        private int packedInstancedFloatCountPixel;

        public List<NuShaderUserTextureInfo> PixelShaderUserTexturesInfo;
        private int EditorAlphaMode;

        public string ShaderFXSize;

        public byte[] Padding = new byte[6];

        public override void Handle(SchemaSerializer schema, uint parentVersion)
        {
            HandleShaderDesc(schema);
            HandleShaderParams(schema);
            
            if (Version > 0xf6)
            {
                schema.HandlePascalString(ref ShaderFXSize, 1);
            }
            
            schema.HandlePascalString(ref MaterialName, 1);
            schema.HandleUInt(ref Flags);
            Debug.Assert(Flags == 4, "flags != 4");

            schema.HandleArray(ref DummyHashArray, 0x494);

            if (schema.Writing)
            {
                VertexLayout.Write(schema.File);
            }
            else
            {
                VertexLayout = VertexList.Parse(schema.File);
            }

            if (Version < 0xda)
            {
                schema.HandleArray(ref Padding, 6);
            }

            HandleMtlAttrib(schema);
            HandleMtlExtra(schema);
        }

        public void Parse(RawFile file)
        {
            SchemaSerializer temp = new SchemaSerializer(file, false);

            HandleShaderDesc(temp);
            HandleShaderParams(temp);
            if (Version > 0xf6)
            {
                short shaderFxSize = file.ReadShort(true); // This is just a pascal string lol
                ShaderFxCode = file.ReadArray(shaderFxSize);
            }
            MaterialName = file.ReadPascalString(true);
            Flags = file.ReadUInt(true);
            Debug.Assert(Flags == 4, "flags != 4");

            DummyHashArray = file.ReadArray(0x494);
            VertexLayout = VertexList.Parse(file);

            if (Version < 0xda)
            {
                for (int i = 0; i < 6; i++)
                {
                    byte c = file.ReadByte();
                }
            }

            HandleMtlAttrib(temp);
            HandleMtlExtra(temp);

            //file.Seek(0x49, SeekOrigin.Current);
        }

        //public override void Write(RawFile file)
        //{

        //    HandleShaderDesc(file, true);
        //    HandleShaderParams(file, Version, true);
        //    file.WritePascalString(MaterialName, 1);
        //    file.WriteUInt(Flags, true);

        //    file.WriteArray(DummyHashArray);
        //    VertexLayout.Write(file);

        //    if (Version < 0xda)
        //    {
        //        file.WriteArray(new byte[6]);
        //    }

        //    HandleMtlAttrib(file, true);
        //    HandleMtlExtra(file, true);
        //}

        public void HandleMtlAttrib(SchemaSerializer schema)
        {
            if (Version < 0xd8)
            {
                schema.HandleByte(ref DefunctOldAlpha);

                schema.HandleByte(ref DefunctOldAtst);
            }
            schema.HandleByte(ref Afail);
            schema.HandleByte(ref Aref);
            schema.HandleByte(ref Cull);
            schema.HandleByte(ref Zmode);
            schema.HandleByte(ref StencilMode);
            schema.HandleByte(ref Noprepass);
            schema.HandleByte(ref Filter);
            schema.HandleByte(ref OldUtc);
            schema.HandleByte(ref OldVtc);
            schema.HandleByte(ref Colour);
            if (Version < 0x100)
            {
                schema.HandleByte(ref OldFillAttrib);
            }
            if (Version < 0xd8)
            {
                schema.HandleByte(ref DefunctOnly2D);
            }
            if (Version < 0xd9)
            {
                schema.HandleByte(ref DefunctStencilShadows);
            }
            schema.HandleByte(ref Castshadow);
            schema.HandleByte(ref OldAutoStencil);
            schema.HandleByte(ref ColourWriteMask);
            if (Version > 0x85)
            {
                schema.HandleByte(ref AlwaysUpdateRefraction);
            }
            if (Version > 0x93)
            {
                schema.HandleByte(ref SortLast);
            }
            if (Version > 0xbd)
            {
                schema.HandleByte(ref ExternalFixupTarget);
            }
            if (Version > 0xca)
            {
                schema.HandleByte(ref SortFirst);
                schema.HandleByte(ref DisableEdgeOutlines);
            }
            if (Version > 0xe4)
            {
                schema.HandleByte(ref PreserveColoursDuringMainFilter);
            }
            schema.HandleByte(ref AlphaTestMode);
        }

        public void HandleMtlExtra(SchemaSerializer schema)
        {
            schema.HandleUInt(ref Fx1);
            schema.HandleUInt(ref Fx2);
            schema.HandleUInt(ref Fx3);
            schema.HandleUInt(ref Fx4);

            if (Version < 0xfb)
            {
                schema.HandleInt(ref OldTid);
            }

            schema.HandleByte(ref Fxid);
            schema.HandleByte(ref SpecialId);

            schema.HandleShort(ref ShortPril16bit);

            if (Version > 0xf9)
            {
                schema.HandleByte(ref isReferencedMaterial);
                schema.HandlePascalString(ref referencedGscName, 1);
                schema.HandlePascalString(ref referencedMaterialName, 1);
            }

            if (Version > 0xf8)
            {
                // do something??
                int link1 = 0;
                schema.HandleInt(ref link1);
                Debug.Assert(link1 == 0);
                int link2 = 0;
                schema.HandleInt(ref link2);
                if (link2 != 0)
                {
                    int link3 = 0;
                    schema.HandleInt(ref link3);
                }

            }
            else
            {
                schema.HandleUInt(ref FirstVariantIdx);
                schema.HandleUInt(ref NextVariantIdx);
            }

            if (Version < 0x10e)
            {
                schema.HandleByte(ref DefunctIsCreasedMeshMaterial);
            }
            schema.HandleByte(ref HasVariants);

            schema.HandleByte(ref LegoStudMaterial);

            schema.HandleByte(ref MaskShadows);
            schema.HandleByte(ref SortAfterRefraction);
            schema.HandleByte(ref SkipValidation);

            schema.HandleByte(ref SpecialDepthSorting);
            schema.HandleByte(ref ForceAlphaLightingSupport);

            if (Version > 0x108)
            {
                schema.HandleByte(ref AlphaRespondToLights);
                schema.HandleByte(ref AlphaRespondToProbes);
            }

            schema.HandleByte(ref NoAutoScreenDoor);
            schema.HandleByte(ref CompileLiveCubemapGenShader);
            if (Version < 0x103)
            {
                schema.HandleByte(ref CompileToonShader);
            }

            schema.HandleByte(ref ShadowImpostor);
            schema.HandleByte(ref ShadowFromFrontFaces);
            schema.HandleByte(ref DoUntexturedTPage);
            schema.HandleByte(ref ForceTPageRemap);
            schema.HandleByte(ref ForceTPageSurfType);
            schema.HandleByte(ref ForceTPageAlphaFade);

            schema.HandleUInt(ref DefaultRenderStage);

            if (Version > 0xfb)
            {
                if (Version > 0x105)
                {
                    if (Version > 0x115)
                    {
                        schema.HandleSchemaVector(ref VertexFixupData);
                    }
                    schema.HandleSchemaVector(ref PixelFixupData);
                }
            }

            if (Version > 0x103)
            {
                if (Version > 0x115)
                {
                    schema.HandleSchemaVector(ref VertexShaderConsts);
                }
                schema.HandleSchemaVector(ref PixelShaderConsts);

                if (Version > 0x105)
                {
                    if (Version > 0x115)
                    {
                        schema.HandleInt(ref packedFloatCountVertex);
                    }

                    schema.HandleInt(ref packedFloatCountPixel);

                    if (Version > 0x114)
                    {
                        if (Version != 0x115)
                        {
                            schema.HandleSchemaVector(ref VertexShaderInstancedConsts);
                            schema.HandleInt(ref packedInstancedFloatCountVertex);
                        }
                        schema.HandleSchemaVector(ref PixelShaderInstancedConsts);
                        schema.HandleInt(ref packedInstancedFloatCountPixel);
                    }
                    if (Version > 0x115)
                    {
                        // TODO: loads of rubbish to implement here
                    }
                }

                schema.HandleSchemaVector(ref PixelShaderUserTexturesInfo);

                if (Version > 0x10c)
                {
                    schema.HandleInt(ref EditorAlphaMode);

                    if (Version > 0x10e)
                    {

                    }
                }
            }
        }

        public void HandleShaderDesc(SchemaSerializer schema)
        {
            int iVar1 = 0;

            if (Version > 0x100)
            {
                schema.HandleUInt(ref ExtraStarter);
            }

            if (Version < 0x10f)
            {
                schema.HandleUInt(ref shader_Version);
            }

            if (Version > 0xe9 && Version < 0x10f)
            {
                schema.HandleShort(ref legoVersion);
            }

            schema.HandleUInt(ref shaderType);
            schema.HandleUInt(ref lightingModel);
            schema.HandleUInt(ref substanceMode);

            if (Version != 0xe7)
            {
                schema.HandleUInt(ref roughnessMode);
            }

            schema.HandleUInt(ref fresnelAlphaMode);

            // Maybe:
            schema.HandleUInt(ref blendMode);
            schema.HandleUInt(ref alphaTest);
            if (Version < 0x10e)
            {
                schema.HandleUInt(ref alphaFadeSource);
            }

            schema.HandleUInt(ref surfaceMapMethod);
            schema.HandleUInt(ref surfaceMapFormat0);
            schema.HandleUInt(ref surfaceMapFormat1);
            schema.HandleUInt(ref surfaceMapFormat2);
            schema.HandleUInt(ref surfaceMapFormat3);
            schema.HandleUInt(ref surfaceMapFormatVTFN);
            schema.HandleUInt(ref occlusion);
            schema.HandleUInt(ref refraction);
            schema.HandleUInt(ref reflection);
            schema.HandleUInt(ref baseDiffuseUsage);
            if (Version > 0xf5)
            {
                schema.HandleUInt(ref RimLightBlendMode);
            }
            schema.HandleUInt(ref layerBlendDiffuse);
            schema.HandleUInt(ref layerBlendDiffuse1);
            schema.HandleUInt(ref layerBlendDiffuse2);
            schema.HandleUInt(ref usesDiffuseLayerColour);
            schema.HandleUInt(ref usesDiffuseLayerColour1);
            schema.HandleUInt(ref usesDiffuseLayerColour2);
            schema.HandleUInt(ref usesDiffuseLayerColour3);

            schema.HandleUInt(ref layerBlendSpecular0);
            schema.HandleUInt(ref layerBlendSpecular1);
            schema.HandleUInt(ref layerBlendSpecular2);
            schema.HandleUInt(ref dummy);
            schema.HandleUInt(ref layerBlendNormal0);
            schema.HandleUInt(ref layerBlendNormal1);
            schema.HandleUInt(ref layerBlendNormal2);
            schema.HandleUInt(ref dummyX);

            schema.HandleUInt(ref numUVSets);
            schema.HandleInt(ref LightmapUVSet);
            if (Version < 0xf5)
            {
                schema.HandleUInt(ref motionBlurVertexType);
                schema.HandleUInt(ref motionBlurPixelType);

                schema.HandleByte(ref motionBlurSamples);
            }

            schema.HandleByte(ref numBones);

            int uvBlocksToRead = 17;
            if (Version > 0xef)
            {
                uvBlocksToRead = 21;
            }

            if (Version > 0x104)
            {
                if (Version < 0x116)
                {
                    uvBlocksToRead += 0x15;
                }
            }

            if (!schema.Writing)
            {
                uvBlocks = new NuMtlUVBlock[uvBlocksToRead];
                for (int i = 0; i < uvBlocks.Length; i++)
                {
                    uvBlocks[i] = new NuMtlUVBlock();
                }
            }

            for (int i = 0; i < uvBlocks.Length; i++)
            {
                schema.HandleUInt(ref uvBlocks[i].State);
                schema.HandleUInt(ref uvBlocks[i].UVSet);

                //Console.WriteLine("--- NEXT:");
                //Console.WriteLine(uvBlocks[i].State);
                //Console.WriteLine(uvBlocks[i].UVSet);
            }

            if (Version < 0xe0)
            {
                schema.HandleByte(ref old_bitangentFlip);
            }

            if (Version < 0x10e)
            {
                schema.HandleByte(ref materialFlags_tangentSwap);
                schema.HandleByte(ref materialFlags_water);
            }

            if (Version > 0xec)
            {
                schema.HandleByte(ref materialFlags_parallaxBlendFix);
            }

            if (Version < 0xe0)
            {
                schema.HandleByte(ref old_nextgenshine);
            }

            schema.HandleByte(ref materialFlags_glow);
            schema.HandleByte(ref materialFlags_carpaint);

            if (Version < 0xe0)
            {
                schema.HandleByte(ref old_fractalbump);
                schema.HandleByte(ref old_fractalbump2);
            }

            schema.HandleByte(ref materialFlags_fog);
            schema.HandleByte(ref materialFlags_unlitNonSRGB);
            schema.HandleByte(ref materialFlags_hdralpha_diffuse);

            if (Version > 0xf6)
            {
                schema.HandleUInt(ref shaderFxCodeHash);
            }

            if (Version < 0x10e)
            {
                schema.HandleByte(ref materialFlags_hdralpha_envmap);
            }

            if (Version < 0x100)
            {
                schema.HandleByte(ref materialFlags_derivHeightMap);
            }
            else
            {
                schema.HandleByte(ref materialFlags_shaderGraphMtl);
            }

            if (Version < 0xe6)
            {
                schema.HandleByte(ref materialFlags_smoothSpec);
            }

            if (Version >= 0xec && Version < 0x10e)
            {
                schema.HandleByte(ref materialFlags_zeusCompatMode);
            }

            schema.HandleByte(ref materialFlags_disable_varying_specular);
            schema.HandleByte(ref materialFlags_disable_fresnel);
            schema.HandleByte(ref materialFlags_two_sided_lighting);
            schema.HandleByte(ref materialFlags_smoothlightmap);
            if (Version < 0x10f)
            {
                schema.HandleByte(ref materialFlags_rimlight);
                schema.HandleByte(ref materialFlags_ignore_exposure);
            }
            schema.HandleByte(ref materialFlags_baked_specular);
            schema.HandleByte(ref materialFlags_semi_lit);
            schema.HandleByte(ref materialFlags_refractionNearFix);
            schema.HandleByte(ref materialFlags_metallic_specular);
            schema.HandleByte(ref materialFlags_dontreceiveshadow);
            schema.HandleByte(ref materialFlags_lateshader);
            schema.HandleByte(ref materialFlags_diffreflmaps);
            schema.HandleByte(ref materialFlags_per_layer_uvscale);
            schema.HandleByte(ref materialFlags_tintable);
            schema.HandleByte(ref materialFlags_generateCubeMap);
            if (Version > 0x111)
            {
                schema.HandleByte(ref materialFlags_depthOnly);
            }
            if (Version > 0x82 && Version < 0x103)
            {
                schema.HandleByte(ref materialFlags_outputToonShaderData);
            }
            if (Version < 0x8b || Version > 0x10e)
            {
                if (Version > 0x118)
                {
                    schema.HandleByte(ref vertexFlags_gameFiveBitPacking);
                }
            }
            else
            {
                schema.HandleByte(ref materialFlags_disablePerPixelFade);
            }
            schema.HandleByte(ref materialFlags_cel_shading);

            if (Version > 0xd6)
            {
                schema.HandleByte(ref miscFlags_conditional_cel_shading);
            }

            if (Version > 0xee)
            {
                schema.HandleByte(ref materialFlags_receiveShadowDespiteCelShading);
            }

            if (Version > 0xde)
            {
                schema.HandleByte(ref miscFlags_useRoomProjection);
            }

            if (Version > 0xdd)
            {
                schema.HandleByte(ref miscFlags_useCustomPixelClipPlane);
            }

            if (Version > 0xe0)
            {
                schema.HandleByte(ref miscFlags_layer2Refraction);
                schema.HandleByte(ref miscFlags_layer3Refraction);
                schema.HandleByte(ref miscFlags_layer4Refraction);
            }

            if (Version > 0xf0)
            {
                schema.HandleByte(ref miscFlags_layer2DX11Only);
                schema.HandleByte(ref miscFlags_layer3DX11Only);
                schema.HandleByte(ref miscFlags_layer4DX11Only);
            }

            if (Version > 0x100)
            {
                schema.HandleByte(ref materialFlags_IsScratchedLego);
            }

            if (Version > 0x102 && Version < 0x10e)
            {
                schema.HandleByte(ref dummy_isMayaShader);
            }

            schema.HandleByte(ref miscFlags_allLayerVertAlbedo);

            schema.HandleByte(ref vertexFlags_skinned);
            schema.HandleByte(ref vertexFlags_fastBlend);
            schema.HandleByte(ref vertexFlags_blendShape);
            schema.HandleByte(ref vertexFlags_doPerspDivInVS);
            schema.HandleByte(ref vertexFlags_numAlphaLayers);
            schema.HandleByte(ref vertexFlags_use2DW);
            schema.HandleByte(ref vertexFlags_untransformed);
            schema.HandleByte(ref vertexFlags_effectAmplitude);
            schema.HandleByte(ref vertexFlags_ignoreVertexOpacity);
            schema.HandleByte(ref vertexFlags_unused1);
            schema.HandleByte(ref vertexFlags_instancedLightmapping);
            schema.HandleByte(ref vertexFlags_positionAccuracy);
            schema.HandleByte(ref vertexFlags_uvAccuracy);
            schema.HandleByte(ref vertexFlags_tangent2);
            schema.HandleByte(ref VertexFlags_VertexControlledTint);
            schema.HandleByte(ref vertexFlags_ZBias);
            schema.HandleByte(ref vertexFlags_layer1VertAlbedo);
            schema.HandleByte(ref vertexFlags_layer2VertAlbedo);
            schema.HandleByte(ref vertexFlags_layer3VertAlbedo);
            schema.HandleByte(ref vertexFlags_disableSeparatePositionStream);
            schema.HandleByte(ref vertexFlags_legoTerrain);
            schema.HandleByte(ref vertexFlags_legoTerrainMeshType);

            if (Version > 0xdb)
            {
                schema.HandleByte(ref vertexFlags_largeWorldAwareCamera);
            }

            schema.HandleByte(ref vertexFlags_wind);

            if (Version > 0xe1)
            {
                schema.HandleByte(ref vertexFlags_forceColourVertexStream);
            }

            if (Version > 0xed)
            {
                schema.HandleByte(ref vertexFlags_vertexRoughnessMod);
            }

            schema.HandleLong(ref vertexFlags_unused2);

            if (Version < 0x10e)
            {
                schema.HandleByte(ref miscFlags_greyAlbedo);
                schema.HandleByte(ref miscFlags_motionBlur);
                schema.HandleByte(ref miscFlags_UVAnimation);
            }

            if (Version < 0xf2)
            {
                schema.HandleByte(ref miscFlags_canAlphaBlend);
            }

            if (iVar1 != 2)
            {
                if (Version < 0x10e)
                {
                    schema.HandleByte(ref miscFlags_defunctOpaque);
                }
                if (Version < 0x10f)
                {
                    schema.HandleByte(ref miscFlags_isDecal);
                }
                if (Version < 0x10e)
                {
                    schema.HandleByte(ref miscFlags_creaseMeshMaterial);
                }
            }

            if (Version > 0x1f && Version < 0x103)
            {
                schema.HandleByte(ref miscFlags_ttAnimationMode);
            }
            schema.HandleByte(ref miscFlags_culled);
            if (Version < 0x10f)
            {
                schema.HandleByte(ref miscFlags_isDeferredDecal);
            }
            if (Version < 0x10e)
            {
                schema.HandleByte(ref miscFlags_isPBRSourced);
            }
            if (Version < 0x10e)
            {
                schema.HandleByte(ref miscFlags_requiresDiffuseAlphaMultiply);
            }
            if (Version < 0x10f)
            {
                schema.HandleByte(ref miscFlags_isTPaged);
            }
            schema.HandleByte(ref miscFlags_disableDynamicLighting);
            schema.HandleByte(ref miscFlags_useLayers234OnWii);
            schema.HandleByte(ref miscFlags_useWiiTintColours);
            schema.HandleByte(ref miscFlags_sRGBSupport);
            if (Version > 0xaa && Version < 0x103)
            {
                schema.HandleByte(ref miscFlags_useNormalEncodingTexture);
            }
            schema.HandleByte(ref miscFlags_refractionIgnoreVertexNormal);
            schema.HandleByte(ref miscFlags_shadedGlow);
            schema.HandleByte(ref miscFlags_project_to_far_plane);
            if (Version < 0x10f)
            {
                schema.HandleByte(ref miscFlags_sortAfterPostEffects);
            }
            if (Version > 0x112)
            {
                schema.HandleByte(ref vertexFlags_usesInstancing);
            }

            schema.HandleByte(ref output_colourRT);
            schema.HandleByte(ref output_normalRT);
            schema.HandleByte(ref output_albedoRT);
            if (Version > 0x10b)
            {
                schema.HandleByte(ref output_tangentRT);
            }
            if (Version > 0x110)
            {
                schema.HandleByte(ref output_emissionRT);
            }
            if (Version < 0x103)
            {
                schema.HandleByte(ref output_depthAsColourRT);
            }

            // Only active when iVar1 == 2?
            if (iVar1 == 2)
            {
                schema.HandleUInt(ref displayMode);
                // schema.HandleByte(ref grassLayers);
            }
            // Only active when iVar1 == 2?

            if (Version < 0x10e)
            {
                schema.HandleUInt(ref shaderVersion);
            }
            schema.HandleUInt(ref gpuVendor);
            schema.HandleUInt(ref colourSpace);
            if (Version < 0x110)
            {
                schema.HandleUInt(ref bakedLighting);
            }

            schema.HandleInt(ref discreteLightType);
            schema.HandleInt(ref discreteLightShadingModel);
            schema.HandleByte(ref discreteLightSoftShadows);

            if (Version < 0xdd)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (!schema.Writing)
                    {
                        DiscreteLight2[i] = new NuMtlDiscreteLight();
                    }

                    schema.HandleInt(ref DiscreteLight2[i].Type);
                    schema.HandleInt(ref DiscreteLight2[i].ShadingModel);
                    schema.HandleByte(ref DiscreteLight2[i].SoftShadows);
                }
            }

            if ((refraction == 2 || iVar1 != 2) && Version < 0x100) // the version < 0x100 is just a guess, I'm not really sure, this seems to be an annoying debug leftover maybe?
            {
                schema.HandleInt(ref sceneZAccess);
            }

            if (Version < 0x10e)
            {
                schema.HandleInt(ref shadowZAccess);
                schema.HandleInt(ref pcfMethod);
                // schema.HandleInt(ref glowMode);
                schema.HandleInt(ref rainSplashSurfaceType);
            }
        }

        public void HandleShaderParams(SchemaSerializer schema)
        {

            schema.HandleInt(ref Diffuse0Index);
            schema.HandleInt(ref Diffuse1Index);
            schema.HandleInt(ref Diffuse2Index);
            schema.HandleInt(ref Diffuse3Index);

            schema.HandleInt(ref Specular0Index);
            schema.HandleInt(ref Specular1Index);

            schema.HandleInt(ref Normal0Index);
            schema.HandleInt(ref Normal1Index);

            schema.HandleInt(ref EnvMap);

            schema.HandleInt(ref VTFH);
            schema.HandleInt(ref VTFN);
            schema.HandleInt(ref DiffEnv);

            schema.HandleInt(ref TexAnimMapTID);
            schema.HandleInt(ref TexAnimCurvesTID);

            schema.HandleInt(ref Normal2);
            schema.HandleInt(ref Specular2);

            schema.HandleInt(ref Normal3);
            schema.HandleInt(ref Specular3);

            if (Version > 0xef)
            {
                schema.HandleInt(ref Detail0);
                schema.HandleInt(ref Detail1);
                schema.HandleInt(ref Detail2);
                schema.HandleInt(ref Detail3);

                schema.HandleFloat(ref DetailRepeat0);
                schema.HandleFloat(ref DetailRepeat1);
                schema.HandleFloat(ref DetailRepeat2);
                schema.HandleFloat(ref DetailRepeat3);
            }

            int numTexAuxEntries = schema.Writing ? TexAuxEntries.Length : 0;

            schema.HandleInt(ref numTexAuxEntries);

            Console.WriteLine($"Reading through {numTexAuxEntries}");

            if (!schema.Writing)
            {
                TexAuxEntries = new NuTexAuxEntry[numTexAuxEntries];
                for (int i = 0; i < TexAuxEntries.Length; i++)
                {
                    TexAuxEntries[i] = new NuTexAuxEntry();
                }
            }

            for (int i = 0; i < TexAuxEntries.Length; i++)
            {
                if (Version < 0xdb)
                {
                    schema.HandleInt(ref TexAuxEntries[i].MaxAnisotropy);
                }
                else
                {
                    byte maxAnisotopry = (byte)(schema.Writing ? TexAuxEntries[i].MaxAnisotropy : 0);
                    schema.HandleByte(ref maxAnisotopry);
                    TexAuxEntries[i].MaxAnisotropy = maxAnisotopry;
                }
            }

            for (int i = 0; i < numTexAuxEntries; i++)
            {
                schema.HandleFloat(ref TexAuxEntries[i].MipmapBias);
            }

            for (int i = 0; i < numTexAuxEntries; i++)
            {
                schema.HandleInt(ref TexAuxEntries[i].TexAuxData);
            }

            schema.HandleInt(ref TexAnimData1);
            schema.HandleInt(ref TexAnimData2);
            schema.HandleInt(ref TexAnimData3);
            schema.HandleInt(ref TexAnimData4);

            for (int i = 0; i < 4; i++)
            {
                TexAnimBlocks[i].Handle(schema);
            }

            schema.HandleInt(ref Colour1);
            schema.HandleInt(ref Colour2);
            schema.HandleInt(ref Colour3);
            schema.HandleInt(ref Colour4);

            schema.HandleByte(ref BitangentFlip);

            schema.HandleFloat(ref KNormal0);
            schema.HandleFloat(ref KNormal1);
            schema.HandleFloat(ref KNormal2);
            schema.HandleFloat(ref KNormal3);

            schema.HandleFloat(ref KParallax);
            schema.HandleFloat(ref KParallaxBias);

            schema.HandleInt(ref Colour5);
            schema.HandleInt(ref Colour6);
            schema.HandleInt(ref Colour7);
            schema.HandleInt(ref Colour8);
            schema.HandleInt(ref Colour9);
            schema.HandleInt(ref Colour10);
            schema.HandleInt(ref Colour11);
            schema.HandleInt(ref Colour12);

            schema.HandleFloat(ref KRefractiveIndex);
            schema.HandleFloat(ref KRefractiveThicknessFactor);
            schema.HandleFloat(ref KGlow);

            schema.HandleInt(ref Colour13);

            if (Version < 0x107)
            {
                schema.HandleFloat(ref KBaseReflectivity);
                schema.HandleFloat(ref KBaseSpecularCosPower);
            }

            schema.HandleFloat(ref KCustomEnvMapStrength);

            if (Version < 0xe7)
            {
                schema.HandleFloat(ref KEnvLighting);
            }

            schema.HandleFloat(ref KEnvAlphaHDR);
            schema.HandleFloat(ref KAlphaFresnelConst);
            schema.HandleFloat(ref KAlphaFresnelPower);
            schema.HandleFloat(ref KVTFHeight);
            schema.HandleFloat(ref KVTFNormal);
            schema.HandleInt(ref KVTFOffset);

            schema.HandleFloat(ref KVTFDirection);
            schema.HandleFloat(ref KVTFDirection1);
            schema.HandleFloat(ref KVTFDirection2);
            schema.HandleFloat(ref KUseVTFDirection);

            schema.HandleInt(ref Vtfh2);

            schema.HandleInt(ref Colour14);
            schema.HandleInt(ref Colour15);

            if (Version < 0xdb)
            {
                schema.HandleInt(ref Colour16);
                schema.HandleInt(ref Colour17);
            }

            schema.HandleFloat(ref KCarPaintViewFactor);
            if (Version < 0xf5)
            {
                schema.HandleFloat(ref KCarPaintLightFactor);
            }
            if (Version < 0x107)
            {
                schema.HandleFloat(ref KBaseRoughness);
            }

            if (Version < 0xe0)
            {
                schema.HandleFloat(ref DefunctKBRDFAnotherSetting);
                schema.HandleFloat(ref DefunctKFractalFrequency);
                schema.HandleFloat(ref DefunctKFractalDiffuse);
                schema.HandleFloat(ref DefunctKFractalSpecular);
                schema.HandleFloat(ref DefunctKFractalLacunarity);
                schema.HandleFloat(ref DefunctKFractalGain);
                schema.HandleFloat(ref DefunctKFractalHeight);
            }

            if (Version < 0xf5)
            {
                schema.HandleFloat(ref KEnvLightIntensity);
            }
            schema.HandleFloat(ref KEnvLightSpecular);
            if (Version < 0xf5)
            {
                schema.HandleFloat(ref KEnvFresnel);
            }
            schema.HandleFloat(ref KEnvFresnelPower);

            if (Version < 0xe6)
            {
                schema.HandleFloat(ref KSpecularBump);
                schema.HandleFloat(ref KSkinSpread);
            }

            if (Version < 0x107)
            {
                schema.HandleFloat(ref KBaseSubstance);
            }

            schema.HandleByte(ref KDepthBias);

            schema.HandleFloat(ref KDepthBiasScale);
            schema.HandleFloat(ref KShadowBias);

            if (Version > 0xde)
            {
                schema.HandleFloat(ref KRoomWidth);
                schema.HandleFloat(ref KRoomHeight);
                schema.HandleFloat(ref KRoomDepth);
            }

            schema.HandleFloat(ref KWiiMaxAlphaBias);

            schema.HandleInt(ref Colour18);
            schema.HandleInt(ref Colour19);

            schema.HandleShort(ref KTPageID);

            schema.HandleFloat(ref KStiffness);

            if (Version >= 0xd1)
            {
                schema.HandleFloat(ref PerLayerUVScale1);
                schema.HandleFloat(ref PerLayerUVScale2);
                schema.HandleFloat(ref PerLayerUVScale3);
                schema.HandleFloat(ref PerLayerUVScale4);

                schema.HandleByte(ref KLightmapTranslucency);
                schema.HandleByte(ref KLightmapEmission);
            }

            if (Version > 0xe3)
            {
                schema.HandleByte(ref BForceDefaultCubeMap);
            }

            if (Version > 0xff)
            {
                if (Version < 0x116)
                {
                    if (Version < 0x106)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            schema.HandleFloat(ref ShaderGraphParams[i]);
                        }
                    }
                    else
                    {
                        schema.HandleSerializableVector(ref ShaderGraphParamsVector);

                        //// Some sort of list structure, need to find some implementations for it!
                        //int test = 0;
                        //schema.HandleInt(ref test);
                        //Debug.Assert(test == 0);
                        //int test2 = 0;
                        //schema.HandleInt(ref test2);
                        //Debug.Assert(test2 == 0);
                    }
                }
            }
        }
    }
}
