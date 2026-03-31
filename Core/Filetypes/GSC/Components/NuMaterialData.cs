using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.Marshalling;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public abstract class NuMaterialData
    {
        public abstract void Parse(RawFile file);

        public abstract void Write(RawFile file);

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

        public (uint, uint)[] uvBlocks;
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

        public static NuMaterialData[] Read(RawFile file)
        {
            Debug.Assert(file.ReadString(4) == "LTMU");
            uint version = file.ReadUInt(true);
            uint count = file.ReadUInt(true);
            NuMaterialData[] materials = new NuMaterialData[count];

            for (int i = 0; i < count; i++)
            {
                NuMaterialData materialData;
                switch (version)
                {
                    case 0xd5:
                    case 0xd6:
                    case 0xd7:
                    case 0xd8:
                    case 0xd9:
                    case 0xda:
                    case 0xdb:
                    case 0xdc:
                    case 0xdd:
                    case 0xde:
                    case 0xdf:
                    case 0xe0:
                    case 0xe1:
                    case 0xe2:
                    case 0xe4:
                        materialData = new NuMaterialData_E0();
                        break;
                    case 0xe5:
                    case 0xe8:
                    case 0xea:
                    case 0xeb:
                    case 0xec:
                        materialData = new NuMaterialData_E5();
                        break;
                    case 0xef:
                    case 0xf0:
                    case 0xf1:
                    case 0xf2:
                        materialData = new NuMaterialData_F2();
                        break;
                    default:
                        throw new Exception($"Unsupported UMTL Version: {version}");
                }

                materialData.Version = version;
                materialData.Parse(file);

                materials[i] = materialData;
            }

            return materials;
        }
    }

    public class NuMaterialData_F2 : NuMaterialData_E5
    {
        public override void Parse(RawFile file)
        {
            ReadShaderDesc(file);
            HandleShaderParams(file, Version, false);
            string materialName = file.ReadPascalString(true);
            uint flags = file.ReadUInt(true);
            Debug.Assert(flags == 4, "flags != 4");

            file.Seek(0x494, SeekOrigin.Current); // "dummyHashArray" (really this should be 0x498, but the VertexList needs to read the prior int I think?)
            VertexList.Parse(file);

            file.Seek(0x4a, SeekOrigin.Current);
        }
    }

    public class NuMaterialData_E5 : NuMaterialData_E0
    {
        public override void Parse(RawFile file)
        {
            base.Parse(file);
            //file.Seek(0x1, SeekOrigin.Current);
        }
    }

    public class NuMaterialData_E0 : NuMaterialData
    {

        public override void Write(RawFile file)
        {
            WriteShaderDesc(file);
            HandleShaderParams(file, Version, true);
            file.WritePascalString(MaterialName, 1);
            file.WriteUInt(Flags, true);

            file.WriteArray(DummyHashArray);
            VertexLayout.Write(file);

            if (Version < 0xda)
            {
                file.WriteArray(new byte[6]);
            }

            HandleMtlAttrib(file, true);
            HandleMtlExtra(file, true);
        }

        public string MaterialName;
        public uint Flags;

        public byte[] DummyHashArray;

        public VertexList VertexLayout;

        public override void Parse(RawFile file)
        {
            ReadShaderDesc(file);
            HandleShaderParams(file, Version, false);
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

            HandleMtlAttrib(file, false);
            HandleMtlExtra(file, false);

            //file.Seek(0x49, SeekOrigin.Current);
        }

        public void HandleMtlAttrib(RawFile file, bool writing)
        {
            SchemaSerializer schema = new SchemaSerializer(file, writing);

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
            schema.HandleByte(ref OldFillAttrib);
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

        public void HandleMtlExtra(RawFile file, bool writing)
        {
            SchemaSerializer schema = new SchemaSerializer(file, writing);

            schema.HandleUInt(ref Fx1);
            schema.HandleUInt(ref Fx2);
            schema.HandleUInt(ref Fx3);
            schema.HandleUInt(ref Fx4);

            schema.HandleInt(ref OldTid);

            schema.HandleByte(ref Fxid);
            schema.HandleByte(ref SpecialId);

            schema.HandleShort(ref ShortPril16bit);

            schema.HandleUInt(ref FirstVariantIdx);
            schema.HandleUInt(ref NextVariantIdx);

            schema.HandleByte(ref DefunctIsCreasedMeshMaterial);
            schema.HandleByte(ref HasVariants);

            schema.HandleByte(ref LegoStudMaterial);

            schema.HandleByte(ref MaskShadows);
            schema.HandleByte(ref SortAfterRefraction);
            schema.HandleByte(ref SkipValidation);

            schema.HandleByte(ref SpecialDepthSorting);
            schema.HandleByte(ref ForceAlphaLightingSupport);

            schema.HandleByte(ref NoAutoScreenDoor);
            schema.HandleByte(ref CompileLiveCubemapGenShader);
            schema.HandleByte(ref CompileToonShader);

            schema.HandleByte(ref ShadowImpostor);
            schema.HandleByte(ref ShadowFromFrontFaces);
            schema.HandleByte(ref DoUntexturedTPage);
            schema.HandleByte(ref ForceTPageRemap);
            schema.HandleByte(ref ForceTPageSurfType);
            schema.HandleByte(ref ForceTPageAlphaFade);

            schema.HandleUInt(ref DefaultRenderStage);
        }

        public void WriteShaderDesc(RawFile file)
        {
            int iVar1 = 0;

            file.WriteUInt(shader_Version, true);

            if (Version > 0xe9)
            {
                file.WriteShort(legoVersion, true);
            }

            file.WriteUInt(shaderType, true);
            file.WriteUInt(lightingModel, true);
            file.WriteUInt(substanceMode, true);

            if (Version != 0xe7)
            {
                file.WriteUInt(roughnessMode, true);
            }

            file.WriteUInt(fresnelAlphaMode, true);

            // Maybe:
            file.WriteUInt(blendMode, true);
            file.WriteUInt(alphaTest, true);
            file.WriteUInt(alphaFadeSource, true);

            file.WriteUInt(surfaceMapMethod, true);
            file.WriteUInt(surfaceMapFormat0, true);
            file.WriteUInt(surfaceMapFormat1, true);
            file.WriteUInt(surfaceMapFormat2, true);
            file.WriteUInt(surfaceMapFormat3, true);
            file.WriteUInt(surfaceMapFormatVTFN, true);
            file.WriteUInt(occlusion, true);
            file.WriteUInt(refraction, true);
            file.WriteUInt(reflection, true);
            file.WriteUInt(baseDiffuseUsage, true);
            file.WriteUInt(layerBlendDiffuse, true);
            file.WriteUInt(layerBlendDiffuse1, true);
            file.WriteUInt(layerBlendDiffuse2, true);
            file.WriteUInt(usesDiffuseLayerColour, true);
            file.WriteUInt(usesDiffuseLayerColour1, true);
            file.WriteUInt(usesDiffuseLayerColour2, true);
            file.WriteUInt(usesDiffuseLayerColour3, true);

            file.WriteUInt(layerBlendSpecular0, true);
            file.WriteUInt(layerBlendSpecular1, true);
            file.WriteUInt(layerBlendSpecular2, true);
            file.WriteUInt(dummy, true);
            file.WriteUInt(layerBlendNormal0, true);
            file.WriteUInt(layerBlendNormal1, true);
            file.WriteUInt(layerBlendNormal2, true);
            file.WriteUInt(dummyX, true);

            file.WriteUInt(numUVSets, true);
            file.WriteInt(LightmapUVSet, true);
            file.WriteUInt(motionBlurVertexType, true);
            file.WriteUInt(motionBlurPixelType, true);

            file.WriteByte(motionBlurSamples);
            file.WriteByte(numBones);

            for (int i = 0; i < uvBlocks.Length; i++)
            {
                (uint state, uint UVSet) = uvBlocks[i];
                file.WriteUInt(state, true);
                file.WriteUInt(UVSet, true);
            }

            if (Version < 0xe0)
            {
                file.WriteByte(old_bitangentFlip);
            }

            file.WriteByte(materialFlags_tangentSwap);
            file.WriteByte(materialFlags_water);

            if (Version > 0xec)
            {
                file.WriteByte(materialFlags_parallaxBlendFix);
            }

            if (Version < 0xe0)
            {
                file.WriteByte(old_nextgenshine);
            }

            file.WriteByte(materialFlags_glow);
            file.WriteByte(materialFlags_carpaint);

            if (Version < 0xe0)
            {
                file.WriteByte(old_fractalbump);
                file.WriteByte(old_fractalbump2);
            }

            file.WriteByte(materialFlags_fog);
            file.WriteByte(materialFlags_unlitNonSRGB);
            file.WriteByte(materialFlags_hdralpha_diffuse);
            file.WriteByte(materialFlags_hdralpha_envmap);
            file.WriteByte(materialFlags_derivHeightMap);

            if (Version < 0xe6)
            {
                file.WriteByte(materialFlags_smoothSpec);
            }

            if (Version >= 0xec)
            {
                file.WriteByte(materialFlags_zeusCompatMode);
            }

            file.WriteByte(materialFlags_disable_varying_specular);
            file.WriteByte(materialFlags_disable_fresnel);
            file.WriteByte(materialFlags_two_sided_lighting);
            file.WriteByte(materialFlags_smoothlightmap);
            file.WriteByte(materialFlags_rimlight);
            file.WriteByte(materialFlags_ignore_exposure);
            file.WriteByte(materialFlags_baked_specular);
            file.WriteByte(materialFlags_semi_lit);
            file.WriteByte(materialFlags_refractionNearFix);
            file.WriteByte(materialFlags_metallic_specular);
            file.WriteByte(materialFlags_dontreceiveshadow);
            file.WriteByte(materialFlags_lateshader);
            file.WriteByte(materialFlags_diffreflmaps);
            file.WriteByte(materialFlags_per_layer_uvscale);
            file.WriteByte(materialFlags_tintable);
            file.WriteByte(materialFlags_generateCubeMap);
            file.WriteByte(materialFlags_outputToonShaderData);
            file.WriteByte(materialFlags_disablePerPixelFade);
            file.WriteByte(materialFlags_cel_shading);

            if (Version > 0xd6)
            {
                file.WriteByte(miscFlags_conditional_cel_shading);
            }

            if (Version > 0xee)
            {
                file.WriteByte(materialFlags_receiveShadowDespiteCelShading);
            }

            if (Version > 0xde)
            {
                file.WriteByte(miscFlags_useRoomProjection);
            }

            if (Version > 0xdd)
            {
                file.WriteByte(miscFlags_useCustomPixelClipPlane);
            }

            if (Version > 0xe0)
            {
                file.WriteByte(miscFlags_layer2Refraction);
                file.WriteByte(miscFlags_layer3Refraction);
                file.WriteByte(miscFlags_layer4Refraction);
            }

            if (Version > 0xf0)
            {
                file.WriteByte(miscFlags_layer2DX11Only);
                file.WriteByte(miscFlags_layer3DX11Only);
                file.WriteByte(miscFlags_layer4DX11Only);
            }

            file.WriteByte(miscFlags_allLayerVertAlbedo);

            file.WriteByte(vertexFlags_skinned);
            file.WriteByte(vertexFlags_fastBlend);
            file.WriteByte(vertexFlags_blendShape);
            file.WriteByte(vertexFlags_doPerspDivInVS);
            file.WriteByte(vertexFlags_numAlphaLayers);
            file.WriteByte(vertexFlags_use2DW);
            file.WriteByte(vertexFlags_untransformed);
            file.WriteByte(vertexFlags_effectAmplitude);
            file.WriteByte(vertexFlags_ignoreVertexOpacity);
            file.WriteByte(vertexFlags_unused1);
            file.WriteByte(vertexFlags_instancedLightmapping);
            file.WriteByte(vertexFlags_positionAccuracy);
            file.WriteByte(vertexFlags_uvAccuracy);
            file.WriteByte(vertexFlags_tangent2);
            file.WriteByte(VertexFlags_VertexControlledTint);
            file.WriteByte(vertexFlags_ZBias);
            file.WriteByte(vertexFlags_layer1VertAlbedo);
            file.WriteByte(vertexFlags_layer2VertAlbedo);
            file.WriteByte(vertexFlags_layer3VertAlbedo);
            file.WriteByte(vertexFlags_disableSeparatePositionStream);
            file.WriteByte(vertexFlags_legoTerrain);
            file.WriteByte(vertexFlags_legoTerrainMeshType);

            if (Version > 0xdb)
            {
                file.WriteByte(vertexFlags_largeWorldAwareCamera);
            }

            file.WriteByte(vertexFlags_wind);

            if (Version > 0xe1)
            {
                file.WriteByte(vertexFlags_forceColourVertexStream);
            }

            if (Version > 0xed)
            {
                file.WriteByte(vertexFlags_vertexRoughnessMod);
            }

            file.WriteLong(0); // padding

            file.WriteByte(miscFlags_greyAlbedo);
            file.WriteByte(miscFlags_motionBlur);
            file.WriteByte(miscFlags_UVAnimation);

            if (Version < 0xf2)
            {
                file.WriteByte(miscFlags_canAlphaBlend);
            }

            if (iVar1 != 2)
            {
                file.WriteByte(miscFlags_defunctOpaque);
                file.WriteByte(miscFlags_isDecal);
                file.WriteByte(miscFlags_creaseMeshMaterial);
            }

            file.WriteByte(miscFlags_ttAnimationMode);
            file.WriteByte(miscFlags_culled);
            file.WriteByte(miscFlags_isDeferredDecal);
            file.WriteByte(miscFlags_isPBRSourced);
            file.WriteByte(miscFlags_requiresDiffuseAlphaMultiply);
            file.WriteByte(miscFlags_isTPaged);
            file.WriteByte(miscFlags_disableDynamicLighting);
            file.WriteByte(miscFlags_useLayers234OnWii);
            file.WriteByte(miscFlags_useWiiTintColours);
            file.WriteByte(miscFlags_sRGBSupport);
            file.WriteByte(miscFlags_useNormalEncodingTexture);
            file.WriteByte(miscFlags_refractionIgnoreVertexNormal);
            file.WriteByte(miscFlags_shadedGlow);
            file.WriteByte(miscFlags_project_to_far_plane);
            file.WriteByte(miscFlags_sortAfterPostEffects);

            file.WriteByte(output_colourRT);
            file.WriteByte(output_normalRT);
            file.WriteByte(output_albedoRT);
            file.WriteByte(output_depthAsColourRT);

            // Only active when iVar1 == 2?
            if (iVar1 == 2)
            {
                file.WriteUInt(displayMode, true);
                // file.WriteByte(grassLayers);
            }
            // Only active when iVar1 == 2?

            file.WriteUInt(shaderVersion, true);
            file.WriteUInt(gpuVendor, true);
            file.WriteUInt(colourSpace, true);
            file.WriteUInt(bakedLighting, true);

            file.WriteInt(discreteLightType, true);
            file.WriteInt(discreteLightShadingModel, true);
            file.WriteByte(discreteLightSoftShadows);

            if (Version < 0xdd)
            {
                throw new NotSupportedException("version < 0xdd not supported for write");
                //for (int i = 0; i < 4; i++)
                //{
                //    file.WriteInt(discreteLight2Type[i], true);
                //    file.WriteInt(discreteLight2ShadingModel[i], true);
                //    file.WriteByte(discreteLight2SoftShadows[i]);
                //}
            }

            if (refraction == 2 || iVar1 != 2)
            {
                file.WriteInt(sceneZAccess, true);
            }

            file.WriteInt(shadowZAccess, true);
            file.WriteInt(pcfMethod, true);
            // file.WriteInt(glowMode, true);
            file.WriteInt(rainSplashSurfaceType, true);
        }

        public void ReadShaderDesc(RawFile file)
        {
            int iVar1 = 0;

            shader_Version = file.ReadUInt(true);
            if (Version > 0xe9)
            {
                legoVersion = file.ReadShort(true);
            }
            shaderType = file.ReadUInt(true);
            lightingModel = file.ReadUInt(true);
            substanceMode = file.ReadUInt(true);
            if (Version != 0xe7)
            {
                roughnessMode = file.ReadUInt(true);
            }
            fresnelAlphaMode = file.ReadUInt(true); // 6

            // Maybe:
            blendMode = file.ReadUInt(true);
            alphaTest = file.ReadUInt(true);
            alphaFadeSource = file.ReadUInt(true); // 3

            surfaceMapMethod = file.ReadUInt(true);
            surfaceMapFormat0 = file.ReadUInt(true);
            surfaceMapFormat1 = file.ReadUInt(true);
            surfaceMapFormat2 = file.ReadUInt(true);
            surfaceMapFormat3 = file.ReadUInt(true);
            surfaceMapFormatVTFN = file.ReadUInt(true);
            occlusion = file.ReadUInt(true);
            refraction = file.ReadUInt(true);
            reflection = file.ReadUInt(true);
            baseDiffuseUsage = file.ReadUInt(true);
            layerBlendDiffuse = file.ReadUInt(true); // (TODO: Should be 3?)
            layerBlendDiffuse1 = file.ReadUInt(true); // (TODO: Should be 3?)
            layerBlendDiffuse2 = file.ReadUInt(true); // (TODO: Should be 3?)
            usesDiffuseLayerColour = file.ReadUInt(true); // 12 (TODO: Should be 4?)
            usesDiffuseLayerColour1 = file.ReadUInt(true); // 12 (TODO: Should be 4?)
            usesDiffuseLayerColour2 = file.ReadUInt(true); // 12 (TODO: Should be 4?)
            usesDiffuseLayerColour3 = file.ReadUInt(true); // 12 (TODO: Should be 4?)

            layerBlendSpecular0 = file.ReadUInt(true);
            layerBlendSpecular1 = file.ReadUInt(true);
            layerBlendSpecular2 = file.ReadUInt(true);
            dummy = file.ReadUInt(true); // It's actually called this, I haven't just tried to re-sync the parser
            layerBlendNormal0 = file.ReadUInt(true);
            layerBlendNormal1 = file.ReadUInt(true);
            layerBlendNormal2 = file.ReadUInt(true); // 6
            dummyX = file.ReadUInt(true); // It's actually called this, I haven't just tried to re-sync the parser

            numUVSets = file.ReadUInt(true);
            LightmapUVSet = file.ReadInt(true);
            motionBlurVertexType = file.ReadUInt(true);
            motionBlurPixelType = file.ReadUInt(true);

            motionBlurSamples = file.ReadByte();
            numBones = file.ReadByte();

            int uvBlocksToRead = 17;
            if (Version > 0xef)
            {
                uvBlocksToRead = 21;
            }
            
            uvBlocks = new (uint, uint)[uvBlocksToRead];

            for (int i = 0; i < uvBlocksToRead; i++)
            {
                uint state = file.ReadUInt(true);
                uint UVSet = file.ReadUInt(true);
                uvBlocks[i] = (state, UVSet);
            }
            if (Version < 0xe0)
            {
                old_bitangentFlip = file.ReadByte();
            }

            materialFlags_tangentSwap = file.ReadByte();
            materialFlags_water = file.ReadByte();
            if (Version > 0xec)
            {
                materialFlags_parallaxBlendFix = file.ReadByte();
            }
            if (Version < 0xe0)
            {
                old_nextgenshine = file.ReadByte();
            }
            materialFlags_glow = file.ReadByte();
            materialFlags_carpaint = file.ReadByte();
            if (Version < 0xe0)
            {
                old_fractalbump = file.ReadByte();
                old_fractalbump2 = file.ReadByte();
            }
            materialFlags_fog = file.ReadByte();
            materialFlags_unlitNonSRGB = file.ReadByte();
            materialFlags_hdralpha_diffuse = file.ReadByte();
            materialFlags_hdralpha_envmap = file.ReadByte();
            materialFlags_derivHeightMap = file.ReadByte();
            if (Version < 0xe6)
            {
                materialFlags_smoothSpec = file.ReadByte();
            }

            if (Version >= 0xec)
            {
                materialFlags_zeusCompatMode = file.ReadByte();
            }

            materialFlags_disable_varying_specular = file.ReadByte();
            materialFlags_disable_fresnel = file.ReadByte();
            materialFlags_two_sided_lighting = file.ReadByte();
            materialFlags_smoothlightmap = file.ReadByte();
            materialFlags_rimlight = file.ReadByte();
            materialFlags_ignore_exposure = file.ReadByte();
            materialFlags_baked_specular = file.ReadByte();
            materialFlags_semi_lit = file.ReadByte();
            materialFlags_refractionNearFix = file.ReadByte();
            materialFlags_metallic_specular = file.ReadByte();
            materialFlags_dontreceiveshadow = file.ReadByte();
            materialFlags_lateshader = file.ReadByte();
            materialFlags_diffreflmaps = file.ReadByte();
            materialFlags_per_layer_uvscale = file.ReadByte();
            materialFlags_tintable = file.ReadByte();
            materialFlags_generateCubeMap = file.ReadByte();
            materialFlags_outputToonShaderData = file.ReadByte();
            materialFlags_disablePerPixelFade = file.ReadByte();
            materialFlags_cel_shading = file.ReadByte(); // 29

            if (Version > 0xd6)
            {
                miscFlags_conditional_cel_shading = file.ReadByte();
            }
            if (Version > 0xee)
            {
                materialFlags_receiveShadowDespiteCelShading = file.ReadByte();
            }
            if (Version > 0xde)
            {
                miscFlags_useRoomProjection = file.ReadByte();
            }
            if (Version > 0xdd)
            {
                miscFlags_useCustomPixelClipPlane = file.ReadByte();
            }
            if (Version > 0xe0)
            {
                miscFlags_layer2Refraction = file.ReadByte();
                miscFlags_layer3Refraction = file.ReadByte();
                miscFlags_layer4Refraction = file.ReadByte();
            }
            if (Version > 0xf0)
            {
                miscFlags_layer2DX11Only = file.ReadByte();
                miscFlags_layer3DX11Only = file.ReadByte();
                miscFlags_layer4DX11Only = file.ReadByte();
            }
            miscFlags_allLayerVertAlbedo = file.ReadByte(); // 7

            vertexFlags_skinned = file.ReadByte();
            vertexFlags_fastBlend = file.ReadByte();
            vertexFlags_blendShape = file.ReadByte();
            vertexFlags_doPerspDivInVS = file.ReadByte();
            vertexFlags_numAlphaLayers = file.ReadByte();
            vertexFlags_use2DW = file.ReadByte();
            vertexFlags_untransformed = file.ReadByte();
            vertexFlags_effectAmplitude = file.ReadByte();
            vertexFlags_ignoreVertexOpacity = file.ReadByte();
            vertexFlags_unused1 = file.ReadByte();
            vertexFlags_instancedLightmapping = file.ReadByte();
            vertexFlags_positionAccuracy = file.ReadByte();
            vertexFlags_uvAccuracy = file.ReadByte();
            vertexFlags_tangent2 = file.ReadByte();
            VertexFlags_VertexControlledTint = file.ReadByte();
            vertexFlags_ZBias = file.ReadByte();
            vertexFlags_layer1VertAlbedo = file.ReadByte();
            vertexFlags_layer2VertAlbedo = file.ReadByte();
            vertexFlags_layer3VertAlbedo = file.ReadByte();
            vertexFlags_disableSeparatePositionStream = file.ReadByte();
            vertexFlags_legoTerrain = file.ReadByte();
            vertexFlags_legoTerrainMeshType = file.ReadByte();
            if (Version > 0xdb)
            {
                vertexFlags_largeWorldAwareCamera = file.ReadByte();
            }
            vertexFlags_wind = file.ReadByte();
            if (Version > 0xe1)
            {
                vertexFlags_forceColourVertexStream = file.ReadByte(); // 25
            }
            if (Version > 0xed)
            {
                vertexFlags_vertexRoughnessMod = file.ReadByte();
            }

            file.ReadLong(); // 8 bytes of zero (It's actually called this, I haven't just tried to re-sync the parser)

            miscFlags_greyAlbedo = file.ReadByte();
            miscFlags_motionBlur = file.ReadByte();
            miscFlags_UVAnimation = file.ReadByte();
            if (Version < 0xf2)
            {
                miscFlags_canAlphaBlend = file.ReadByte();
            }
            if (iVar1 != 2)
            {
                miscFlags_defunctOpaque = file.ReadByte();
                miscFlags_isDecal = file.ReadByte();
                miscFlags_creaseMeshMaterial = file.ReadByte();
            }
            miscFlags_ttAnimationMode = file.ReadByte();
            miscFlags_culled = file.ReadByte();
            miscFlags_isDeferredDecal = file.ReadByte();
            miscFlags_isPBRSourced = file.ReadByte();
            miscFlags_requiresDiffuseAlphaMultiply = file.ReadByte();
            miscFlags_isTPaged = file.ReadByte();
            miscFlags_disableDynamicLighting = file.ReadByte();
            miscFlags_useLayers234OnWii = file.ReadByte();
            miscFlags_useWiiTintColours = file.ReadByte();
            miscFlags_sRGBSupport = file.ReadByte();
            miscFlags_useNormalEncodingTexture = file.ReadByte();
            miscFlags_refractionIgnoreVertexNormal = file.ReadByte();
            miscFlags_shadedGlow = file.ReadByte();
            miscFlags_project_to_far_plane = file.ReadByte();
            miscFlags_sortAfterPostEffects = file.ReadByte(); // 22

            output_colourRT = file.ReadByte();
            output_normalRT = file.ReadByte();
            output_albedoRT = file.ReadByte();
            output_depthAsColourRT = file.ReadByte(); // 4

            // Only active when iVar1 == 2?
            if (iVar1 == 2)
            {
                displayMode = file.ReadUInt(true);
                //byte grassLayers = file.ReadByte();
            }
            // Only active when iVar1 == 2?

            shaderVersion = file.ReadUInt(true);
            gpuVendor = file.ReadUInt(true);
            colourSpace = file.ReadUInt(true);
            bakedLighting = file.ReadUInt(true);

            discreteLightType = file.ReadInt(true);
            discreteLightShadingModel = file.ReadInt(true);
            discreteLightSoftShadows = file.ReadByte();
            if (Version < 0xdd)
            {
                for (int i = 0; i < 4; i++)
                {
                    int discreteLight2Type = file.ReadInt(true); // TODO: Should be an int!
                    int discreteLight2ShadingModel = file.ReadInt(true); // TODO: Should be an int!
                    byte discreteLight2SoftShadows = file.ReadByte();
                }
            }

            if (refraction == 2 || iVar1 != 2)
            {
                sceneZAccess = file.ReadInt(true); // TODO: Should be an int!
            }
            shadowZAccess = file.ReadInt(true);
            pcfMethod = file.ReadInt(true);
            //int glowMode = file.ReadInt(true); // doesn't exist for v > 0x99
            rainSplashSurfaceType = file.ReadInt(true);
        }

        public void HandleShaderParams(RawFile file, uint version, bool writing)
        {
            SchemaSerializer schema = new SchemaSerializer(file, writing);

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

            if (version > 0xef)
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

            int numTexAuxEntries = writing ? TexAuxEntries.Length : 0;

            schema.HandleInt(ref numTexAuxEntries);

            if (!writing)
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
                    byte maxAnisotopry = (byte)(writing ? TexAuxEntries[i].MaxAnisotropy : 0);
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

            schema.HandleFloat(ref KBaseReflectivity);
            schema.HandleFloat(ref KBaseSpecularCosPower);
            schema.HandleFloat(ref KCustomEnvMapStrength);

            if (version < 0xe7)
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
            schema.HandleFloat(ref KCarPaintLightFactor);
            schema.HandleFloat(ref KBaseRoughness);

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

            schema.HandleFloat(ref KEnvLightIntensity);
            schema.HandleFloat(ref KEnvLightSpecular);
            schema.HandleFloat(ref KEnvFresnel);
            schema.HandleFloat(ref KEnvFresnelPower);

            if (Version < 0xe6)
            {
                schema.HandleFloat(ref KSpecularBump);
                schema.HandleFloat(ref KSkinSpread);
            }

            schema.HandleFloat(ref KBaseSubstance);

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
        }
    }
}
