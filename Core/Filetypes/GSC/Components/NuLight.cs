using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuLight : IVectorSerializable
    {
        public virtual void ReadLightDescCommon(RawFile file, uint parentVersion)
        {
            string lightDescName = file.ReadPascalString();
            byte enabled = file.ReadByte();
            int layer = file.ReadInt(true);
            int shapeType = file.ReadInt(true);
            int emitterShape = file.ReadInt(true);
            int shadingModel = file.ReadInt(true);
            int falloff = file.ReadInt(true);
            int importance = file.ReadInt(true);
            int platform = file.ReadInt(true);
            int directDiffuseMode = file.ReadInt(true);
            int bouncedDiffuseMode = file.ReadInt(true);
            int specularMode = file.ReadInt(true);
            int godRays = file.ReadInt(true);
            float effectIntensity = file.ReadFloat(true);
            float godRayFallOff = file.ReadFloat(true);
            byte lensFlare = file.ReadByte();
            int lensFlareShape = file.ReadInt(true);
            float lensFlareCircleAlpha = file.ReadFloat(true);
            float lensFlareCoronaAlpha = file.ReadFloat(true);
            byte lensFlareCircleDoHueShift = file.ReadByte();
            float lensFlareCircleHueShift = file.ReadFloat(true);
            byte lensFlareCoronaDoHueShift = file.ReadByte();
            float lensFlareCoronaHueShift = file.ReadFloat(true);

            int temp = file.ReadInt(true);

            if (parentVersion > 0x36)
            {
                byte lensFlareOverride1Index = file.ReadByte();
                byte lensFlareOverride2Index = file.ReadByte();

                int buffer1 = file.ReadInt(true);
                Debug.Assert(buffer1 == 0, "dlgt buffer non-zero!");

                int buffer2 = file.ReadInt(true);
                Debug.Assert(buffer2 == 0, "dlgt buffer non-zero!");
            }

            int zero = file.ReadInt(true); // possibly to do with a buffer?
            Debug.Assert(zero == 0);
            Vector3 emitterSize = new Vector3(file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true));
            float lensFlareIntensity = file.ReadFloat(true);
            float coronaIntensity = file.ReadFloat(true);
            float godRayIntensity = file.ReadFloat(true);
            Vector3 colour = new Vector3(file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true));
            Vector3 linearColour = new Vector3(file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true));
            Vector3 secondaryColourLinear = new Vector3(file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true));
            Vector3 secondaryColour = new Vector3(file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true));

            int zero2 = file.ReadInt(true); // another buffer i think
            Debug.Assert(zero2 == 0);

            float val1 = file.ReadFloat(true);
            float val2 = file.ReadFloat(true);
            float val3 = file.ReadFloat(true);
            float shadowIntensity = file.ReadFloat(true);
            byte isNegativeLight = file.ReadByte();
            byte lsvHotspot = file.ReadByte();
            float falloffStart = file.ReadFloat(true);
            float spotFarClip = file.ReadFloat(true);
            float lightFalloffStartDx9 = file.ReadFloat(true);
            float lightFalloffRangeDx9 = file.ReadFloat(true);
            if (parentVersion > 0x38)
            {
                float lightFalloffStartDx11 = file.ReadFloat(true);
                float lightFalloffRangeDx11 = file.ReadFloat(true);
            }
            int tid = file.ReadInt(true);
            float spotFOVY_Degrees = file.ReadFloat(true);
            float spotFOVY_Cos = file.ReadFloat(true);
            float spotInnerFOVY_Degrees = file.ReadFloat(true);
            float spotInnerFOVY_Cos = file.ReadFloat(true);
            float spotNearClip = file.ReadFloat(true);
            float falloffFactor = file.ReadFloat(true);
            float spotLinearFalloffRangeFactor = file.ReadFloat(true);
            float spotAspectRatio = file.ReadFloat(true);
            float spotNegativeDistance = file.ReadFloat(true);
            uint zero3 = file.ReadUInt(true);
            Debug.Assert(zero3 == 0); // another buffer?
            uint zero4 = file.ReadUInt(true);
            Debug.Assert(zero4 == 0); // another buffer?
        }

        public virtual void ReadLightDesc(RawFile file, uint parentVersion)
        {
            byte usedBits = file.ReadByte();
            Debug.Assert(usedBits == 0);
            uint numRayTracedShadowRays = file.ReadUInt(true);
            int realTimeMethod = file.ReadInt(true);
            int shadowProjectionMethod = file.ReadInt(true);
            float shadowMapRangeInGame0 = file.ReadFloat(true);
            float shadowMapRangeInGame1 = file.ReadFloat(true);
            float shadowMapRangeInGame2 = file.ReadFloat(true);
            float shadowMapRangeInGame3 = file.ReadFloat(true);
            float shadowMapRangeInGame4 = file.ReadFloat(true);
            float shadowFalloffStartInGameDx9 = file.ReadFloat(true);
            float shadowFalloffRangeInGameDx9 = file.ReadFloat(true);
            if (parentVersion > 0x38)
            {
                float shadowFallOffStartInGameDx11 = file.ReadFloat(true);
                float shadowFallOffRangeInGameDx11 = file.ReadFloat(true);
            }
            float shadowBiasInGame = file.ReadFloat(true);
            int shadowSplitInGame = file.ReadInt(true);
            float shadowMapRangeCutsceneInGame0 = file.ReadFloat(true);
            float shadowMapRangeCutsceneInGame1 = file.ReadFloat(true);
            float shadowMapRangeCutsceneInGame2 = file.ReadFloat(true);
            float shadowMapRangeCutsceneInGame3 = file.ReadFloat(true);
            float shadowMapRangeCutsceneInGame4 = file.ReadFloat(true);
            
            byte haveCustomCutsceneShadowRanges = file.ReadByte();
            float shadowFalloffStartCutsceneDx9 = file.ReadFloat(true);
            float shadowFalloffRangeCutsceneDx9 = file.ReadFloat(true);
            if (parentVersion > 0x38)
            {
                float shadowFalloffStartCutsceneDx11 = file.ReadFloat(true);
                float shadowFalloffRangeCutsceneDx11 = file.ReadFloat(true);
            }
            float shadowBiasCutscene = file.ReadFloat(true);
            int shadowSplitMethodCutscene = file.ReadInt(true);

            byte focusSpotShadowsFromMaya = file.ReadByte();
            byte focusSpotShadowsInGame = file.ReadByte();
            byte focusSpotShadowsCutscenes = file.ReadByte();
            byte castShadowsFromFrontFaces = file.ReadByte();
            byte downwardsCubeShadowsOnly = file.ReadByte();

            float occluderPixelThreshold0 = file.ReadFloat(true);
            float occluderPixelThreshold1 = file.ReadFloat(true);
            float occluderPixelThreshold2 = file.ReadFloat(true);
            float occluderPixelThreshold3 = file.ReadFloat(true);

            byte enableDynamicCascadeSizes = file.ReadByte();
            float dynamicCascadesStartDistance = file.ReadFloat(true);
            float dynamicCascadesEndDistance = file.ReadFloat(true);
            float dynamicCascadesEndFactor = file.ReadFloat(true);

            uint dynamicCascadesCount = file.ReadUInt(true);
            float shadowBiasDoubleSided = file.ReadFloat(true);

            float shadowIntensityFromBaseClass = file.ReadFloat(true);
            byte softShadowsEnabledDx9 = file.ReadByte();

            if (parentVersion > 0x34)
            {
                byte softShadowsEnabledDx11 = file.ReadByte();
                int shadowMapSizeDividerDx9 = file.ReadInt(true);
                int shadowMapSizeDividerDx11 = file.ReadInt(true);
            }

            float softShadowsFixedSpreadInGameDx9 = file.ReadFloat(true);
            if (parentVersion > 0x35)
            {
                float softShadowsFixedSpreadInGameDx11 = file.ReadFloat(true);
            }
            if (parentVersion > 0x37)
            {
                float softShadowsFixedSpreadCutsceneDx9 = file.ReadFloat(true);
                float softShadowsFixedSpreadCutsceneDx11 = file.ReadFloat(true);
            }
            float softShadowsSpreadRatio = file.ReadFloat(true);
            int was_softShadowsSampleCount = file.ReadInt(true);
            float softShadowOverlap = file.ReadFloat(true);
            float softShadowCenterBias = file.ReadFloat(true);
            byte softShadowsSpreadRatioEnabled = file.ReadByte();
            float deprecatedSoftShadowsFixedSpread2 = file.ReadFloat(true);
            byte enableDynamicCascadeSizesInGame = file.ReadByte();
            byte enableDynamicCascadeSizesCutscenes = file.ReadByte();

            byte defunct_ShadowNoiseEnabled = file.ReadByte();
            float defunct_ShadowNoiseScale = file.ReadFloat(true);
            float defunct_ShadowNoiseIntensity = file.ReadFloat(true);
            byte castShadowsFromSpecials = file.ReadByte();
            byte castShadowsFromTerrain = file.ReadByte();
            byte onlyCastShadowsIfPlayerIsInVolume = file.ReadByte();

            byte shadowsOnDx11Only = file.ReadByte(); // v > 0x33

            byte deferredBoundingBoxesAsExclusion = file.ReadByte();
            byte useBoundingBoxesForBlendedDirectional = file.ReadByte();

            float stencilShadowExtrusionYPlane = file.ReadFloat(true);
            float stencilShadowExtrusionMaxLength = file.ReadFloat(true);
            byte usesLightRestrictionBox = file.ReadByte();
        }

        public void Deserialize(RawFile file, uint parentVersion)
        {
            ReadLightDescCommon(file, parentVersion);
            ReadLightDesc(file, parentVersion);

            NuMtx mtx = new NuMtx();
            mtx.Deserialize(file, parentVersion);
        }
    }
}
