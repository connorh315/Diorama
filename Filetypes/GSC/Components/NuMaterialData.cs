using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Filetypes.GSC.Components
{
    public abstract class NuMaterialData
    {
        public abstract void Parse(RawFile file);

        public uint Version;

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
                    case 0xe0:
                    case 0xe2:
                    case 0xe4:
                        materialData = new NuMaterialData_E0();
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

            file.Seek(0x49, SeekOrigin.Current);
        }

        public void ReadShaderDesc(RawFile file)
        {
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
            uint layerBlendDiffuse = file.ReadUInt(true);
            uint usesDiffuseLayerColour = file.ReadUInt(true); // 12

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

            uint dummy2 = file.ReadUInt(true);
            uint dummy3 = file.ReadUInt(true);
            uint dummy4 = file.ReadUInt(true);
            uint dummy5 = file.ReadUInt(true);
            uint dummy6 = file.ReadUInt(true);
            uint dummy7 = file.ReadUInt(true);

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

            byte materialFlags_tangentSwap = file.ReadByte();
            byte materialFlags_water = file.ReadByte();
            if (Version > 0xec)
            {
                byte materialFlags_parallaxBlendFix = file.ReadByte();
            }
            byte materialFlags_glow = file.ReadByte();
            byte materialFlags_carpaint = file.ReadByte();
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

            byte miscFlags_conditional_cel_shading = file.ReadByte();
            if (Version > 0xee)
            {
                byte materialFlags_receiveShadowDespiteCelShading = file.ReadByte();
            }
            byte miscFlags_useRoomProjection = file.ReadByte();
            byte miscFlags_useCustomPixelClipPlane = file.ReadByte();
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
            byte vertexFlags_largeWorldAwareCamera = file.ReadByte();
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
            byte miscFlags_defunctOpaque = file.ReadByte();
            byte miscFlags_isDecal = file.ReadByte();
            byte miscFlags_creaseMeshMaterial = file.ReadByte();
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

            uint displayMode = file.ReadUInt(true);
            uint grassLayers = file.ReadUInt(true);
            uint shaderVersion = file.ReadUInt(true);
            uint gpuVendor = file.ReadUInt(true);
            uint colourSpace = file.ReadUInt(true);
            uint bakedLighting = file.ReadUInt(true);

            byte discreteLightType = file.ReadByte();
            byte discreteLightShadingModel = file.ReadByte();
            byte discreteLightSoftShadows = file.ReadByte();
            byte blank = file.ReadByte();
            byte sceneZAccess = file.ReadByte();
            byte shadowZAccess = file.ReadByte();
            byte pcfMethod = file.ReadByte();
            byte rainSplashSurfaceType = file.ReadByte();

            uint unknown = file.ReadUInt(true); // This might be engine hash
            byte unknown1 = file.ReadByte();
        }

        private static void ReadShaderParams(RawFile file, uint version)
        {
            int diffuse0 = file.ReadInt(true);
            int diffuse1 = file.ReadInt(true);
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

            file.Seek(numTexAuxEntries * 9, SeekOrigin.Current);
            file.Seek(0x10, SeekOrigin.Current); // Not sure what this section does either - Always 0xff

            //int maxAnisotropy = file.ReadInt(true);
            //int mipmapBias = file.ReadInt(true);

            for (int i = 0; i < 4; i++)
            {
                byte modeU = file.ReadByte();
                byte modeV = file.ReadByte();

                int dU = file.ReadInt(true);
                int dV = file.ReadInt(true);
                int speedU = file.ReadInt(true);
                int speedV = file.ReadInt(true);

                byte ssNumColumns = file.ReadByte();
                byte ssNumRows = file.ReadByte();
                byte ssRowIndex = file.ReadByte();
                byte ssNumImages = file.ReadByte();

                float ssDuration = file.ReadFloat(true);
                byte ssOffset = file.ReadByte();
            }

            byte colourA = file.ReadByte();
            byte colourB = file.ReadByte();
            byte colourG = file.ReadByte();
            byte colourR = file.ReadByte();

            int colour2 = file.ReadInt(true);
            int colour3 = file.ReadInt(true);
            int colour4 = file.ReadInt(true);

            byte bitangentFlip = file.ReadByte();

            float kNormal0 = file.ReadFloat(true);
            float kNormal1 = file.ReadFloat(true);
            float kNormal2 = file.ReadFloat(true);
            float kNormal3 = file.ReadFloat(true);

            float kParallax = file.ReadFloat(true);
            int kParallaxBias = file.ReadInt(true);

            int colour5 = file.ReadInt(true);
            int colour6 = file.ReadInt(true);
            int colour7 = file.ReadInt(true);
            int colour8 = file.ReadInt(true);
            int colour9 = file.ReadInt(true);
            int colour10 = file.ReadInt(true);
            int colour11 = file.ReadInt(true);
            int colour12 = file.ReadInt(true);

            int kRefractiveIndex = file.ReadInt(true);
            int kRefractiveThicknessFactor = file.ReadInt(true);
            int kGlow = file.ReadInt(true);

            int colour13 = file.ReadInt(true);

            float kBaseReflectivity = file.ReadFloat(true);
            float kBaseSpecularCosPower = file.ReadFloat(true);
            float kCustomEnvMapStrength = file.ReadFloat(true);
            if (version < 0xe7)
            {
                float kEnvLighting = file.ReadFloat(true);
            }
            int kEnvAlphaHDR = file.ReadInt(true);
            float kAlphaFresnelConst = file.ReadFloat(true);
            float kAlphaFresnelPower = file.ReadFloat(true);
            float kVTFHeight = file.ReadFloat(true);
            float kVTFNormal = file.ReadFloat(true);
            int kVTFOffset = file.ReadInt(true);

            int kVTFDirection = file.ReadInt(true);
            int kVTFDirection1 = file.ReadInt(true);
            int kVTFDirection2 = file.ReadInt(true);
            int kUseVTFDirection = file.ReadInt(true);

            int colour14 = file.ReadInt(true);
            int colour15 = file.ReadInt(true);

            int kCarPaintViewFactor = file.ReadInt(true);
            float kCarPaintLightFactor = file.ReadFloat(true);
            float kBaseRoughness = file.ReadFloat(true);

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
            float extra = file.ReadFloat(true);

            byte kDepthBias = file.ReadByte();

            float kDepthBiasScale = file.ReadFloat(true);
            float kShadowBias = file.ReadFloat(true);
            float kRoomWidth = file.ReadFloat(true);
            float kRoomHeight = file.ReadFloat(true);
            float kRoomDepth = file.ReadFloat(true);
            float kWiiMaxAlphaBias = file.ReadFloat(true);

            int colour16 = file.ReadInt(true);
            int colour17 = file.ReadInt(true);

            short kTPageID = file.ReadShort(true);

            float kStiffness = file.ReadFloat(true);

            float perLayerUVScale1 = file.ReadFloat(true);
            float perLayerUVScale2 = file.ReadFloat(true);
            float perLayerUVScale3 = file.ReadFloat(true);
            float perLayerUVScale4 = file.ReadFloat(true);

            byte kLightmapTranslucency = file.ReadByte();
            byte kLightmapEmission = file.ReadByte();
            if (version > 0xe3)
            {
                byte bForceDefaultCubeMap = file.ReadByte();
            }
        }
    }
}
