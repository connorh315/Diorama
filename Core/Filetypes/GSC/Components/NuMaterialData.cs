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

        public uint Version;

        public int Diffuse0Index;
        public int Diffuse1Index;

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
            ReadShaderParams(file, Version);
            string materialName = file.ReadPascalString(true);
            uint flags = file.ReadUInt(true);
            Debug.Assert(flags == 4);

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
        public override void Parse(RawFile file)
        {
            ReadShaderDesc(file);
            ReadShaderParams(file, Version);
            string materialName = file.ReadPascalString(true);
            uint flags = file.ReadUInt(true);
            Debug.Assert(flags == 4);
            
            file.Seek(0x494, SeekOrigin.Current); // "dummyHashArray"
            VertexList.Parse(file);

            if (Version < 0xda)
            {
                for (int i = 0; i < 6; i++)
                {
                    byte c = file.ReadByte();
                }
            }

            ReadMtlAttrib(file);
            uint fx1 = file.ReadUInt(true);
            uint fx2 = file.ReadUInt(true);
            uint fx3 = file.ReadUInt(true);
            uint fx4 = file.ReadUInt(true);

            int old_tid = file.ReadInt(true);

            byte fxid = file.ReadByte();
            byte specialId = file.ReadByte();

            short shortPril16bit = file.ReadShort(true);

            uint firstVariantIdx = file.ReadUInt(true);
            uint nextVariantIdx = file.ReadUInt(true);

            byte defunct_isCreasedMeshMaterial = file.ReadByte();
            byte hasVariants = file.ReadByte();

            byte legoStudMaterial = file.ReadByte();

            byte maskShadows = file.ReadByte();
            byte sortAfterRefraction = file.ReadByte();
            byte skipValidation = file.ReadByte();

            byte specialDepthSorting = file.ReadByte();
            byte forceAlphaLightingSupport = file.ReadByte();

            byte noAutoScreenDoor = file.ReadByte();
            byte compileLiveCubemapGenShader = file.ReadByte();
            byte compileToonShader = file.ReadByte();

            byte shadowImpostor = file.ReadByte();
            byte shadowFromFrontFaces = file.ReadByte();
            byte doUntexturedTPage = file.ReadByte();
            byte forceTPageRemap = file.ReadByte();
            byte forceTPageSurfType = file.ReadByte();
            byte forceTPageAlphaFade = file.ReadByte();

            uint defaultRenderStage = file.ReadUInt(true);

            //file.Seek(0x49, SeekOrigin.Current);
        }

        public void ReadMtlAttrib(RawFile file)
        {
            if (Version < 0xd8)
            {
                byte defunctOldAlpha = file.ReadByte();
                byte defunctOldAtst = file.ReadByte();
            }
            byte afail = file.ReadByte();
            byte aref = file.ReadByte();
            byte cull = file.ReadByte();
            byte zmode = file.ReadByte();
            byte stencilMode = file.ReadByte();
            byte noprepass = file.ReadByte();
            byte filter = file.ReadByte();
            byte old_utc = file.ReadByte();
            byte old_vtc = file.ReadByte();
            byte colour = file.ReadByte();
            byte oldFillAttrib = file.ReadByte();
            if (Version < 0xd8)
            {
                byte defunctOnly2D = file.ReadByte();
            }
            if (Version < 0xd9)
            {
                byte defunctStencilShadows = file.ReadByte();
            }
            byte castshadow = file.ReadByte();
            byte old_autoStencil = file.ReadByte();
            byte colourWriteMask = file.ReadByte();
            if (Version > 0x85)
            {
                byte alwaysUpdateRefraction = file.ReadByte();
            }
            if (Version > 0x93)
            {
                byte sortLast = file.ReadByte();
            }
            if (Version > 0xbd)
            {
                byte externalFixupTarget = file.ReadByte();
            }
            if (Version > 0xca)
            {
                byte sortFirst = file.ReadByte();
                byte disableEdgeOutlines = file.ReadByte();
            }
            if (Version > 0xe4)
            {
                byte preserveColoursDuringMainFilter = file.ReadByte();
            }
            byte alphaTestMode = file.ReadByte();
        }

        public void ReadShaderDesc(RawFile file)
        {
            int iVar1 = 0;

            uint shader_Version = file.ReadUInt(true);
            if (Version > 0xe9)
            {
                short legoVersion = file.ReadShort(true);
            }
            uint shaderType = file.ReadUInt(true);
            uint lightingModel = file.ReadUInt(true);
            uint substanceMode = file.ReadUInt(true);
            if (Version != 0xe7)
            {
                uint roughnessMode = file.ReadUInt(true);
            }
            uint fresnelAlphaMode = file.ReadUInt(true); // 6

            // Maybe:
            uint blendMode = file.ReadUInt(true);
            uint alphaTest = file.ReadUInt(true);
            uint alphaFadeSource = file.ReadUInt(true); // 3

            uint surfaceMapMethod = file.ReadUInt(true);
            uint surfaceMapFormat0 = file.ReadUInt(true);
            uint surfaceMapFormat1 = file.ReadUInt(true);
            uint surfaceMapFormat2 = file.ReadUInt(true);
            uint surfaceMapFormat3 = file.ReadUInt(true);
            uint surfaceMapFormatVTFN = file.ReadUInt(true);
            uint occlusion = file.ReadUInt(true);
            uint refraction = file.ReadUInt(true);
            uint reflection = file.ReadUInt(true);
            uint baseDiffuseUsage = file.ReadUInt(true);
            uint layerBlendDiffuse = file.ReadUInt(true); // (TODO: Should be 3?)
            uint layerBlendDiffuse1 = file.ReadUInt(true); // (TODO: Should be 3?)
            uint layerBlendDiffuse2 = file.ReadUInt(true); // (TODO: Should be 3?)
            uint usesDiffuseLayerColour = file.ReadUInt(true); // 12 (TODO: Should be 4?)
            uint usesDiffuseLayerColour1 = file.ReadUInt(true); // 12 (TODO: Should be 4?)
            uint usesDiffuseLayerColour2 = file.ReadUInt(true); // 12 (TODO: Should be 4?)
            uint usesDiffuseLayerColour3 = file.ReadUInt(true); // 12 (TODO: Should be 4?)

            uint layerBlendSpecular0 = file.ReadUInt(true);
            uint layerBlendSpecular1 = file.ReadUInt(true);
            uint layerBlendSpecular2 = file.ReadUInt(true);
            uint dummy = file.ReadUInt(true); // It's actually called this, I haven't just tried to re-sync the parser
            uint layerBlendNormal0 = file.ReadUInt(true);
            uint layerBlendNormal1 = file.ReadUInt(true);
            uint layerBlendNormal2 = file.ReadUInt(true); // 6
            uint dummyX = file.ReadUInt(true); // It's actually called this, I haven't just tried to re-sync the parser

            uint numUVSets = file.ReadUInt(true);
            uint lightmapUVSet = file.ReadUInt(true);
            uint motionBlurVertexType = file.ReadUInt(true);
            uint motionBlurPixelType = file.ReadUInt(true);

            //uint dummy2 = file.ReadUInt(true);
            //uint dummy3 = file.ReadUInt(true);
            //uint dummy4 = file.ReadUInt(true);
            //uint dummy5 = file.ReadUInt(true);
            //uint dummy6 = file.ReadUInt(true);
            //uint dummy7 = file.ReadUInt(true);

            byte motionBlurSamples = file.ReadByte();
            byte numBones = file.ReadByte();

            int uvBlocksToRead = 17;
            if (Version > 0xef)
            {
                uvBlocksToRead = 21;
            }
            for (int i = 0; i < uvBlocksToRead; i++)
            {
                uint state = file.ReadUInt(true);
                uint UVSet = file.ReadUInt(true);
            }
            if (Version < 0xe0)
            {
                byte old_bitangentFlip = file.ReadByte();
            }

            byte materialFlags_tangentSwap = file.ReadByte();
            byte materialFlags_water = file.ReadByte();
            if (Version > 0xec)
            {
                byte materialFlags_parallaxBlendFix = file.ReadByte();
            }
            if (Version < 0xe0)
            {
                byte old_nextgenshine = file.ReadByte();
            }
            byte materialFlags_glow = file.ReadByte();
            byte materialFlags_carpaint = file.ReadByte();
            if (Version < 0xe0)
            {
                byte old_fractalbump = file.ReadByte();
                byte old_fractalbump2 = file.ReadByte();
            }
            byte materialFlags_fog = file.ReadByte();
            byte materialFlags_unlitNonSRGB = file.ReadByte();
            byte materialFlags_hdralpha_diffuse = file.ReadByte();
            byte materialFlags_hdralpha_envmap = file.ReadByte();
            byte materialFlags_derivHeightMap = file.ReadByte();
            if (Version < 0xe6)
            {
                byte materialFlags_smoothSpec = file.ReadByte();
            }

            if (Version >= 0xec)
            {
                byte materialFlags_zeusCompatMode = file.ReadByte();
            }

            byte materialFlags_disable_varying_specular = file.ReadByte();
            byte materialFlags_disable_fresnel = file.ReadByte();
            byte materialFlags_two_sided_lighting = file.ReadByte();
            byte materialFlags_smoothlightmap = file.ReadByte();
            byte materialFlags_rimlight = file.ReadByte();
            byte materialFlags_ignore_exposure = file.ReadByte();
            byte materialFlags_baked_specular = file.ReadByte();
            byte materialFlags_semi_lit = file.ReadByte();
            byte materialFlags_refractionNearFix = file.ReadByte();
            byte materialFlags_metallic_specular = file.ReadByte();
            byte materialFlags_dontreceiveshadow = file.ReadByte();
            byte materialFlags_lateshader = file.ReadByte();
            byte materialFlags_diffreflmaps = file.ReadByte();
            byte materialFlags_per_layer_uvscale = file.ReadByte();
            byte materialFlags_tintable = file.ReadByte();
            byte materialFlags_generateCubeMap = file.ReadByte();
            byte materialFlags_outputToonShaderData = file.ReadByte();
            byte materialFlags_disablePerPixelFade = file.ReadByte();
            byte materialFlags_cel_shading = file.ReadByte(); // 29

            if (Version > 0xd6)
            {
                byte miscFlags_conditional_cel_shading = file.ReadByte();
            }
            if (Version > 0xee)
            {
                byte materialFlags_receiveShadowDespiteCelShading = file.ReadByte();
            }
            if (Version > 0xde)
            {
                byte miscFlags_useRoomProjection = file.ReadByte();
            }
            if (Version > 0xdd)
            {
                byte miscFlags_useCustomPixelClipPlane = file.ReadByte();
            }
            if (Version > 0xe0)
            {
                byte miscFlags_layer2Refraction = file.ReadByte();
                byte miscFlags_layer3Refraction = file.ReadByte();
                byte miscFlags_layer4Refraction = file.ReadByte();
            }
            if (Version > 0xf0)
            {
                byte miscFlags_layer2DX11Only = file.ReadByte();
                byte miscFlags_layer3DX11Only = file.ReadByte();
                byte miscFlags_layer4DX11Only = file.ReadByte();
            }
            byte miscFlags_allLayerVertAlbedo = file.ReadByte(); // 7

            byte vertexFlags_skinned = file.ReadByte();
            byte vertexFlags_fastBlend = file.ReadByte();
            byte vertexFlags_blendShape = file.ReadByte();
            byte vertexFlags_doPerspDivInVS = file.ReadByte();
            byte vertexFlags_numAlphaLayers = file.ReadByte();
            byte vertexFlags_use2DW = file.ReadByte();
            byte vertexFlags_untransformed = file.ReadByte();
            byte vertexFlags_effectAmplitude = file.ReadByte();
            byte vertexFlags_ignoreVertexOpacity = file.ReadByte();
            byte vertexFlags_unused1 = file.ReadByte();
            byte vertexFlags_instancedLightmapping = file.ReadByte();
            byte vertexFlags_positionAccuracy = file.ReadByte();
            byte vertexFlags_uvAccuracy = file.ReadByte();
            byte vertexFlags_tangent2 = file.ReadByte();
            byte vertexFlags_vertexControlledTint = file.ReadByte();
            byte vertexFlags_ZBias = file.ReadByte();
            byte vertexFlags_layer1VertAlbedo = file.ReadByte();
            byte vertexFlags_layer2VertAlbedo = file.ReadByte();
            byte vertexFlags_layer3VertAlbedo = file.ReadByte();
            byte vertexFlags_disableSeparatePositionStream = file.ReadByte();
            byte vertexFlags_legoTerrain = file.ReadByte();
            byte vertexFlags_legoTerrainMeshType = file.ReadByte();
            if (Version > 0xdb)
            {
                byte vertexFlags_largeWorldAwareCamera = file.ReadByte();
            }
            byte vertexFlags_wind = file.ReadByte();
            if (Version > 0xe1)
            {
                byte vertexFlags_forceColourVertexStream = file.ReadByte(); // 25
            }
            if (Version > 0xed)
            {
                byte vertexFlags_vertexRoughnessMod = file.ReadByte();
            }

            file.ReadLong(); // 8 bytes of zero (It's actually called this, I haven't just tried to re-sync the parser)
            // TODO: Definitely something wrong after this point. Too much information I think.
            byte miscFlags_greyAlbedo = file.ReadByte();
            byte miscFlags_motionBlur = file.ReadByte();
            byte miscFlags_UVAnimation = file.ReadByte();
            if (Version < 0xf2)
            {
                byte miscFlags_canAlphaBlend = file.ReadByte();
            }
            if (iVar1 != 2)
            {
                byte miscFlags_defunctOpaque = file.ReadByte();
                byte miscFlags_isDecal = file.ReadByte();
                byte miscFlags_creaseMeshMaterial = file.ReadByte();
            }
            byte miscFlags_ttAnimationMode = file.ReadByte();
            byte miscFlags_culled = file.ReadByte();
            byte miscFlags_isDeferredDecal = file.ReadByte();
            byte miscFlags_isPBRSourced = file.ReadByte();
            byte miscFlags_requiresDiffuseAlphaMultiply = file.ReadByte();
            byte miscFlags_isTPaged = file.ReadByte();
            byte miscFlags_disableDynamicLighting = file.ReadByte();
            byte miscFlags_useLayers234OnWii = file.ReadByte();
            byte miscFlags_useWiiTintColours = file.ReadByte();
            byte miscFlags_sRGBSupport = file.ReadByte();
            byte miscFlags_useNormalEncodingTexture = file.ReadByte();
            byte miscFlags_refractionIgnoreVertexNormal = file.ReadByte();
            byte miscFlags_shadedGlow = file.ReadByte();
            byte miscFlags_project_to_far_plane = file.ReadByte();
            byte miscFlags_sortAfterPostEffects = file.ReadByte(); // 22

            byte output_colourRT = file.ReadByte();
            byte output_normalRT = file.ReadByte();
            byte output_albedoRT = file.ReadByte();
            byte output_depthAsColourRT = file.ReadByte(); // 4

            // Only active when iVar1 == 2?
            if (iVar1 == 2)
            {
                uint displayMode = file.ReadUInt(true);
                byte grassLayers = file.ReadByte();
            }
            // Only active when iVar1 == 2?

            uint shaderVersion = file.ReadUInt(true);
            uint gpuVendor = file.ReadUInt(true);
            uint colourSpace = file.ReadUInt(true);
            uint bakedLighting = file.ReadUInt(true);

            int discreteLightType = file.ReadInt(true);
            int discreteLightShadingModel = file.ReadInt(true);
            byte discreteLightSoftShadows = file.ReadByte();
            if (Version < 0xdd)
            {
                for (int i = 0; i < 4; i++)
                {
                    int discreteLight2Type = file.ReadInt(true); // TODO: Should be an int!
                    int discreteLight2ShadingModel = file.ReadInt(true); // TODO: Should be an int!
                    byte discreteLIght2SoftShadows = file.ReadByte();
                }
            }

            if (refraction == 2 || iVar1 != 2)
            {
                int sceneZAccess = file.ReadInt(true); // TODO: Should be an int!
            }
            int shadowZAccess = file.ReadInt(true);
            int pcfMethod = file.ReadInt(true);
            //int glowMode = file.ReadInt(true); // doesn't exist for v > 0x99
            int rainSplashSurfaceType = file.ReadInt(true);
        }

        public void ReadShaderParams(RawFile file, uint version)
        {
            Diffuse0Index = file.ReadInt(true);
            Diffuse1Index = file.ReadInt(true);
            int diffuse2 = file.ReadInt(true);
            int diffuse3 = file.ReadInt(true);

            int specular0 = file.ReadInt(true);
            int specular1 = file.ReadInt(true);

            int normal0 = file.ReadInt(true);
            int normal1 = file.ReadInt(true);

            int envMap = file.ReadInt(true);

            int VTFH = file.ReadInt(true);
            int VTFN = file.ReadInt(true);
            int diffEnv = file.ReadInt(true);
            int texAnimMapTID = file.ReadInt(true);
            int texAnimCurvesTID = file.ReadInt(true);

            int normal2 = file.ReadInt(true);
            int specular2 = file.ReadInt(true);

            int normal3 = file.ReadInt(true);
            int specular3 = file.ReadInt(true);

            if (version > 0xef)
            {
                int detail0 = file.ReadInt(true);
                int detail1 = file.ReadInt(true);
                int detail2 = file.ReadInt(true);
                int detail3 = file.ReadInt(true);

                float detailRepeat0 = file.ReadFloat(true);
                float detailRepeat1 = file.ReadFloat(true);
                float detailRepeat2 = file.ReadFloat(true);
                float detailRepeat3 = file.ReadFloat(true);
            }

            int numTexAuxEntries = file.ReadInt(true); // I think
            for (int i = 0; i < numTexAuxEntries; i++)
            {
                if (Version < 0xdb)
                {
                    int maxAnisotropy = file.ReadInt(true);
                }
                else
                {
                    byte maxAnisotropy = file.ReadByte();
                }
            }

            for (int i = 0; i < numTexAuxEntries; i++)
            {
                float mipmapBias = file.ReadFloat(true);
            }

            for (int i = 0; i < numTexAuxEntries; i++)
            {
                int texAuxData = file.ReadInt(true);
            }

            int texAnimData1 = file.ReadInt(true);
            int texAnimData2 = file.ReadInt(true);
            int texAnimData3 = file.ReadInt(true);
            int texAnimData4 = file.ReadInt(true);

            //file.Seek(numTexAuxEntries * 9, SeekOrigin.Current);
            //file.Seek(0x10, SeekOrigin.Current); // Not sure what this section does either - Always 0xff

            //int maxAnisotropy = file.ReadInt(true);
            //int mipmapBias = file.ReadInt(true);

            for (int i = 0; i < 4; i++)
            {
                byte modeU = file.ReadByte();
                byte modeV = file.ReadByte();

                float dU = file.ReadFloat(true);
                float dV = file.ReadFloat(true);
                float speedU = file.ReadFloat(true);
                float speedV = file.ReadFloat(true);

                byte ssNumColumns = file.ReadByte();
                byte ssNumRows = file.ReadByte();
                byte ssRowIndex = file.ReadByte();
                byte ssNumImages = file.ReadByte();

                float ssDuration = file.ReadFloat(true);
                byte ssOffset = file.ReadByte();
            }

            int colour1 = file.ReadInt(true); // abgr

            int colour2 = file.ReadInt(true);
            int colour3 = file.ReadInt(true);
            int colour4 = file.ReadInt(true);

            byte bitangentFlip = file.ReadByte();

            float kNormal0 = file.ReadFloat(true);
            float kNormal1 = file.ReadFloat(true);
            float kNormal2 = file.ReadFloat(true);
            float kNormal3 = file.ReadFloat(true);

            float kParallax = file.ReadFloat(true);
            float kParallaxBias = file.ReadFloat(true);

            int colour5 = file.ReadInt(true);
            int colour6 = file.ReadInt(true);
            int colour7 = file.ReadInt(true);
            int colour8 = file.ReadInt(true);
            int colour9 = file.ReadInt(true);
            int colour10 = file.ReadInt(true);
            int colour11 = file.ReadInt(true);
            int colour12 = file.ReadInt(true);

            float kRefractiveIndex = file.ReadFloat(true);
            float kRefractiveThicknessFactor = file.ReadFloat(true);
            float kGlow = file.ReadFloat(true);

            int colour13 = file.ReadInt(true);

            float kBaseReflectivity = file.ReadFloat(true);
            float kBaseSpecularCosPower = file.ReadFloat(true);
            float kCustomEnvMapStrength = file.ReadFloat(true);
            if (version < 0xe7)
            {
                float kEnvLighting = file.ReadFloat(true);
            }
            float kEnvAlphaHDR = file.ReadFloat(true);
            float kAlphaFresnelConst = file.ReadFloat(true);
            float kAlphaFresnelPower = file.ReadFloat(true);
            float kVTFHeight = file.ReadFloat(true);
            float kVTFNormal = file.ReadFloat(true);
            int kVTFOffset = file.ReadInt(true);

            float kVTFDirection = file.ReadFloat(true);
            float kVTFDirection1 = file.ReadFloat(true);
            float kVTFDirection2 = file.ReadFloat(true);
            float kUseVTFDirection = file.ReadFloat(true);

            int vtfh2 = file.ReadInt(true);

            int colour14 = file.ReadInt(true);
            int colour15 = file.ReadInt(true);
            if (Version < 0xdb)
            {
                int colour16 = file.ReadInt(true);
                int colour17 = file.ReadInt(true);
            }

            float kCarPaintViewFactor = file.ReadFloat(true);
            float kCarPaintLightFactor = file.ReadFloat(true);
            float kBaseRoughness = file.ReadFloat(true);

            if (Version < 0xe0)
            {
                float defunct_kBRDFAnotherSetting = file.ReadFloat(true);
                float defunct_kFractalFrequency = file.ReadFloat(true);
                float defunct_kFractalDiffuse = file.ReadFloat(true);
                float defunct_kFractalSpecular = file.ReadFloat(true);
                float defunct_kFractalLacunarity = file.ReadFloat(true);
                float defunct_kFractalGain = file.ReadFloat(true);
                float defunct_kFractalHeight = file.ReadFloat(true);
            }

            float kEnvLightIntensity = file.ReadFloat(true);
            float kEnvLightSpecular = file.ReadFloat(true);
            float kEnvFresnel = file.ReadFloat(true);
            float kEnvFresnelPower = file.ReadFloat(true);
            if (version < 0xe6)
            {
                float kSpecularBump = file.ReadFloat(true);
                float kSkinSpread = file.ReadFloat(true);
            }
            float kBaseSubstance = file.ReadFloat(true);

            byte kDepthBias = file.ReadByte();

            float kDepthBiasScale = file.ReadFloat(true);
            float kShadowBias = file.ReadFloat(true);
            if (Version > 0xde)
            {
                float kRoomWidth = file.ReadFloat(true);
                float kRoomHeight = file.ReadFloat(true);
                float kRoomDepth = file.ReadFloat(true);
            }
            float kWiiMaxAlphaBias = file.ReadFloat(true);

            int colour18 = file.ReadInt(true);
            int colour19 = file.ReadInt(true);

            short kTPageID = file.ReadShort(true);

            float kStiffness = file.ReadFloat(true);

            if (Version >= 0xd1)
            {
                float perLayerUVScale1 = file.ReadFloat(true);
                float perLayerUVScale2 = file.ReadFloat(true);
                float perLayerUVScale3 = file.ReadFloat(true);
                float perLayerUVScale4 = file.ReadFloat(true);
                byte kLightmapTranslucency = file.ReadByte();
                byte kLightmapEmission = file.ReadByte();
            }

            if (version > 0xe3)
            {
                byte bForceDefaultCubeMap = file.ReadByte();
            }
        }
    }
}
