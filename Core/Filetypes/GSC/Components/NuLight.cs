using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuLight : ISchemaSerializable
    {
        public string LightDescName;

        public byte Enabled;

        public int Layer;
        public int ShapeType;
        public int EmitterShape;
        public int ShadingModel;
        public int Falloff;
        public int Importance;
        public int Platform;
        public int DirectDiffuseMode;
        public int BouncedDiffuseMode;
        public int SpecularMode;
        public int GodRays;

        public float EffectIntensity;
        public float GodRayFallOff;

        public byte LensFlare;
        public int LensFlareShape;

        public float LensFlareCircleAlpha;
        public float LensFlareCoronaAlpha;

        public byte LensFlareCircleDoHueShift;
        public float LensFlareCircleHueShift;

        public byte LensFlareCoronaDoHueShift;
        public float LensFlareCoronaHueShift;

        public int Temp;

        public byte LensFlareOverride1Index;
        public byte LensFlareOverride2Index;

        public int Buffer1;
        public int Buffer2;

        public int Buffer3;
        
        public Vector3 EmitterSize;

        public float LensFlareIntensity;
        public float CoronaIntensity;
        public float GodRayIntensity;

        public Vector3 Colour;
        public Vector3 LinearColour;
        public Vector3 SecondaryColourLinear;
        public Vector3 SecondaryColour;

        public int Buffer4;

        public float Val1;
        public float Val2;
        public float Val3;

        public float ShadowIntensity;

        public byte IsNegativeLight;
        public byte LsvHotspot;

        public float FalloffStart;
        public float SpotFarClip;

        public float LightFalloffStartDx9;
        public float LightFalloffRangeDx9;

        public float LightFalloffStartDx11; // parentVersion > 0x38
        public float LightFalloffRangeDx11; // parentVersion > 0x38

        public int Tid;

        public float SpotFOVYDegrees;
        public float SpotFOVYCos;
        public float SpotInnerFOVYDegrees;
        public float SpotInnerFOVYCos;

        public float SpotNearClip;
        public float FalloffFactor;
        public float SpotLinearFalloffRangeFactor;
        public float SpotAspectRatio;
        public float SpotNegativeDistance;

        public uint Buffer5;
        public uint Buffer6;

        public void HandleLightDescCommon(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandlePascalString(ref LightDescName, 1);

            schema.HandleByte(ref Enabled);

            schema.HandleInt(ref Layer);
            schema.HandleInt(ref ShapeType);
            schema.HandleInt(ref EmitterShape);
            schema.HandleInt(ref ShadingModel);
            schema.HandleInt(ref Falloff);
            schema.HandleInt(ref Importance);
            schema.HandleInt(ref Platform);
            schema.HandleInt(ref DirectDiffuseMode);
            schema.HandleInt(ref BouncedDiffuseMode);
            schema.HandleInt(ref SpecularMode);
            schema.HandleInt(ref GodRays);

            schema.HandleFloat(ref EffectIntensity);
            schema.HandleFloat(ref GodRayFallOff);

            schema.HandleByte(ref LensFlare);
            schema.HandleInt(ref LensFlareShape);

            schema.HandleFloat(ref LensFlareCircleAlpha);
            schema.HandleFloat(ref LensFlareCoronaAlpha);

            schema.HandleByte(ref LensFlareCircleDoHueShift);
            schema.HandleFloat(ref LensFlareCircleHueShift);

            schema.HandleByte(ref LensFlareCoronaDoHueShift);
            schema.HandleFloat(ref LensFlareCoronaHueShift);

            schema.HandleInt(ref Temp);

            if (parentVersion > 0x36)
            {
                schema.HandleByte(ref LensFlareOverride1Index);
                schema.HandleByte(ref LensFlareOverride2Index);

                schema.HandleInt(ref Buffer1);
                Debug.Assert(Buffer1 == 0, "dlgt buffer non-zero!");

                schema.HandleInt(ref Buffer2);
                Debug.Assert(Buffer2 == 0, "dlgt buffer non-zero!");
            }

            schema.HandleInt(ref Buffer3);
            Debug.Assert(Buffer3 == 0);
            schema.HandleVector3(ref EmitterSize);

            schema.HandleFloat(ref LensFlareIntensity);
            schema.HandleFloat(ref CoronaIntensity);
            schema.HandleFloat(ref GodRayIntensity);

            schema.HandleVector3(ref Colour);
            schema.HandleVector3(ref LinearColour);
            schema.HandleVector3(ref SecondaryColourLinear);
            schema.HandleVector3(ref SecondaryColour);

            schema.HandleInt(ref Buffer4);
            Debug.Assert(Buffer4 == 0);

            schema.HandleFloat(ref Val1);
            schema.HandleFloat(ref Val2);
            schema.HandleFloat(ref Val3);

            schema.HandleFloat(ref ShadowIntensity);

            schema.HandleByte(ref IsNegativeLight);
            schema.HandleByte(ref LsvHotspot);

            schema.HandleFloat(ref FalloffStart);
            schema.HandleFloat(ref SpotFarClip);

            schema.HandleFloat(ref LightFalloffStartDx9);
            schema.HandleFloat(ref LightFalloffRangeDx9);

            if (parentVersion > 0x38)
            {
                schema.HandleFloat(ref LightFalloffStartDx11);
                schema.HandleFloat(ref LightFalloffRangeDx11);
            }

            schema.HandleInt(ref Tid);

            schema.HandleFloat(ref SpotFOVYDegrees);
            schema.HandleFloat(ref SpotFOVYCos);
            schema.HandleFloat(ref SpotInnerFOVYDegrees);
            schema.HandleFloat(ref SpotInnerFOVYCos);

            schema.HandleFloat(ref SpotNearClip);
            schema.HandleFloat(ref FalloffFactor);
            schema.HandleFloat(ref SpotLinearFalloffRangeFactor);
            schema.HandleFloat(ref SpotAspectRatio);
            schema.HandleFloat(ref SpotNegativeDistance);

            schema.HandleUInt(ref Buffer5);
            Debug.Assert(Buffer5 == 0);

            schema.HandleUInt(ref Buffer6);
            Debug.Assert(Buffer6 == 0);
        }

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

        public byte UsedBits;

        public uint NumRayTracedShadowRays;

        public int RealTimeMethod;
        public int ShadowProjectionMethod;

        public float ShadowMapRangeInGame0;
        public float ShadowMapRangeInGame1;
        public float ShadowMapRangeInGame2;
        public float ShadowMapRangeInGame3;
        public float ShadowMapRangeInGame4;

        public float ShadowFalloffStartInGameDx9;
        public float ShadowFalloffRangeInGameDx9;

        public float ShadowFalloffStartInGameDx11; // parentVersion > 0x38
        public float ShadowFalloffRangeInGameDx11; // parentVersion > 0x38

        public float ShadowBiasInGame;
        public int ShadowSplitInGame;

        public float ShadowMapRangeCutsceneInGame0;
        public float ShadowMapRangeCutsceneInGame1;
        public float ShadowMapRangeCutsceneInGame2;
        public float ShadowMapRangeCutsceneInGame3;
        public float ShadowMapRangeCutsceneInGame4;

        public byte HaveCustomCutsceneShadowRanges;

        public float ShadowFalloffStartCutsceneDx9;
        public float ShadowFalloffRangeCutsceneDx9;

        public float ShadowFalloffStartCutsceneDx11; // parentVersion > 0x38
        public float ShadowFalloffRangeCutsceneDx11; // parentVersion > 0x38

        public float ShadowBiasCutscene;
        public int ShadowSplitMethodCutscene;

        public byte FocusSpotShadowsFromMaya;
        public byte FocusSpotShadowsInGame;
        public byte FocusSpotShadowsCutscenes;
        public byte CastShadowsFromFrontFaces;
        public byte DownwardsCubeShadowsOnly;

        public float OccluderPixelThreshold0;
        public float OccluderPixelThreshold1;
        public float OccluderPixelThreshold2;
        public float OccluderPixelThreshold3;

        public byte EnableDynamicCascadeSizes;
        public float DynamicCascadesStartDistance;
        public float DynamicCascadesEndDistance;
        public float DynamicCascadesEndFactor;

        public uint DynamicCascadesCount;
        public float ShadowBiasDoubleSided;

        public float ShadowIntensityFromBaseClass;
        public byte SoftShadowsEnabledDx9;

        public byte SoftShadowsEnabledDx11;        // parentVersion > 0x34
        public int ShadowMapSizeDividerDx9;        // parentVersion > 0x34
        public int ShadowMapSizeDividerDx11;       // parentVersion > 0x34

        public float SoftShadowsFixedSpreadInGameDx9;
        public float SoftShadowsFixedSpreadInGameDx11; // parentVersion > 0x35

        public float SoftShadowsFixedSpreadCutsceneDx9;  // parentVersion > 0x37
        public float SoftShadowsFixedSpreadCutsceneDx11; // parentVersion > 0x37

        public float SoftShadowsSpreadRatio;
        public int WasSoftShadowsSampleCount;
        public float SoftShadowOverlap;
        public float SoftShadowCenterBias;

        public byte SoftShadowsSpreadRatioEnabled;
        public float DeprecatedSoftShadowsFixedSpread2;

        public byte EnableDynamicCascadeSizesInGame;
        public byte EnableDynamicCascadeSizesCutscenes;

        public byte DefunctShadowNoiseEnabled;
        public float DefunctShadowNoiseScale;
        public float DefunctShadowNoiseIntensity;

        public byte CastShadowsFromSpecials;
        public byte CastShadowsFromTerrain;
        public byte OnlyCastShadowsIfPlayerIsInVolume;

        public byte ShadowsOnDx11Only;

        public byte DeferredBoundingBoxesAsExclusion;
        public byte UseBoundingBoxesForBlendedDirectional;

        public float StencilShadowExtrusionYPlane;
        public float StencilShadowExtrusionMaxLength;

        public byte UsesLightRestrictionBox;

        public void HandleLightDesc(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandleByte(ref UsedBits);
            Debug.Assert(UsedBits == 0);

            schema.HandleUInt(ref NumRayTracedShadowRays);

            schema.HandleInt(ref RealTimeMethod);
            schema.HandleInt(ref ShadowProjectionMethod);

            schema.HandleFloat(ref ShadowMapRangeInGame0);
            schema.HandleFloat(ref ShadowMapRangeInGame1);
            schema.HandleFloat(ref ShadowMapRangeInGame2);
            schema.HandleFloat(ref ShadowMapRangeInGame3);
            schema.HandleFloat(ref ShadowMapRangeInGame4);

            schema.HandleFloat(ref ShadowFalloffStartInGameDx9);
            schema.HandleFloat(ref ShadowFalloffRangeInGameDx9);

            if (parentVersion > 0x38)
            {
                schema.HandleFloat(ref ShadowFalloffStartInGameDx11);
                schema.HandleFloat(ref ShadowFalloffRangeInGameDx11);
            }

            schema.HandleFloat(ref ShadowBiasInGame);
            schema.HandleInt(ref ShadowSplitInGame);

            schema.HandleFloat(ref ShadowMapRangeCutsceneInGame0);
            schema.HandleFloat(ref ShadowMapRangeCutsceneInGame1);
            schema.HandleFloat(ref ShadowMapRangeCutsceneInGame2);
            schema.HandleFloat(ref ShadowMapRangeCutsceneInGame3);
            schema.HandleFloat(ref ShadowMapRangeCutsceneInGame4);

            schema.HandleByte(ref HaveCustomCutsceneShadowRanges);

            schema.HandleFloat(ref ShadowFalloffStartCutsceneDx9);
            schema.HandleFloat(ref ShadowFalloffRangeCutsceneDx9);

            if (parentVersion > 0x38)
            {
                schema.HandleFloat(ref ShadowFalloffStartCutsceneDx11);
                schema.HandleFloat(ref ShadowFalloffRangeCutsceneDx11);
            }

            schema.HandleFloat(ref ShadowBiasCutscene);
            schema.HandleInt(ref ShadowSplitMethodCutscene);

            schema.HandleByte(ref FocusSpotShadowsFromMaya);
            schema.HandleByte(ref FocusSpotShadowsInGame);
            schema.HandleByte(ref FocusSpotShadowsCutscenes);
            schema.HandleByte(ref CastShadowsFromFrontFaces);
            schema.HandleByte(ref DownwardsCubeShadowsOnly);

            schema.HandleFloat(ref OccluderPixelThreshold0);
            schema.HandleFloat(ref OccluderPixelThreshold1);
            schema.HandleFloat(ref OccluderPixelThreshold2);
            schema.HandleFloat(ref OccluderPixelThreshold3);

            schema.HandleByte(ref EnableDynamicCascadeSizes);
            schema.HandleFloat(ref DynamicCascadesStartDistance);
            schema.HandleFloat(ref DynamicCascadesEndDistance);
            schema.HandleFloat(ref DynamicCascadesEndFactor);

            schema.HandleUInt(ref DynamicCascadesCount);
            schema.HandleFloat(ref ShadowBiasDoubleSided);

            schema.HandleFloat(ref ShadowIntensityFromBaseClass);
            schema.HandleByte(ref SoftShadowsEnabledDx9);

            if (parentVersion > 0x34)
            {
                schema.HandleByte(ref SoftShadowsEnabledDx11);
                schema.HandleInt(ref ShadowMapSizeDividerDx9);
                schema.HandleInt(ref ShadowMapSizeDividerDx11);
            }

            schema.HandleFloat(ref SoftShadowsFixedSpreadInGameDx9);

            if (parentVersion > 0x35)
            {
                schema.HandleFloat(ref SoftShadowsFixedSpreadInGameDx11);
            }

            if (parentVersion > 0x37)
            {
                schema.HandleFloat(ref SoftShadowsFixedSpreadCutsceneDx9);
                schema.HandleFloat(ref SoftShadowsFixedSpreadCutsceneDx11);
            }

            schema.HandleFloat(ref SoftShadowsSpreadRatio);
            schema.HandleInt(ref WasSoftShadowsSampleCount);
            schema.HandleFloat(ref SoftShadowOverlap);
            schema.HandleFloat(ref SoftShadowCenterBias);

            schema.HandleByte(ref SoftShadowsSpreadRatioEnabled);
            schema.HandleFloat(ref DeprecatedSoftShadowsFixedSpread2);

            schema.HandleByte(ref EnableDynamicCascadeSizesInGame);
            schema.HandleByte(ref EnableDynamicCascadeSizesCutscenes);

            schema.HandleByte(ref DefunctShadowNoiseEnabled);
            schema.HandleFloat(ref DefunctShadowNoiseScale);
            schema.HandleFloat(ref DefunctShadowNoiseIntensity);

            schema.HandleByte(ref CastShadowsFromSpecials);
            schema.HandleByte(ref CastShadowsFromTerrain);
            schema.HandleByte(ref OnlyCastShadowsIfPlayerIsInVolume);

            schema.HandleByte(ref ShadowsOnDx11Only);

            schema.HandleByte(ref DeferredBoundingBoxesAsExclusion);
            schema.HandleByte(ref UseBoundingBoxesForBlendedDirectional);

            schema.HandleFloat(ref StencilShadowExtrusionYPlane);
            schema.HandleFloat(ref StencilShadowExtrusionMaxLength);

            schema.HandleByte(ref UsesLightRestrictionBox);
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

        public NuMtx Mtx = new NuMtx();

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            HandleLightDescCommon(schema, parentVersion);
            HandleLightDesc(schema, parentVersion);

            Mtx.Handle(schema, parentVersion);
        }
    }
}
