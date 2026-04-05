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
        public uint Version;

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

        public void HandleLightDescCommon(SchemaSerializer schema)
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

            if (Version > 0x28)
            {
                schema.HandleByte(ref LensFlare);
            }

            if (Version > 0x31)
            {
                schema.HandleInt(ref LensFlareShape);

                schema.HandleFloat(ref LensFlareCircleAlpha);
                schema.HandleFloat(ref LensFlareCoronaAlpha);

                schema.HandleByte(ref LensFlareCircleDoHueShift);
                schema.HandleFloat(ref LensFlareCircleHueShift);

                schema.HandleByte(ref LensFlareCoronaDoHueShift);
                schema.HandleFloat(ref LensFlareCoronaHueShift);
            }

            if (Version > 0x31)
            {
                schema.HandleInt(ref Temp);

                if (Version > 0x36)
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
            }
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

            if (Version > 0x38)
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

        public void HandleLightDesc(SchemaSerializer schema)
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

            if (Version > 0x38)
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

            if (Version > 0x38)
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

            if (Version > 0x34)
            {
                schema.HandleByte(ref SoftShadowsEnabledDx11);
                schema.HandleInt(ref ShadowMapSizeDividerDx9);
                schema.HandleInt(ref ShadowMapSizeDividerDx11);
            }

            schema.HandleFloat(ref SoftShadowsFixedSpreadInGameDx9);

            if (Version > 0x35)
            {
                schema.HandleFloat(ref SoftShadowsFixedSpreadInGameDx11);
            }

            if (Version > 0x37)
            {
                schema.HandleFloat(ref SoftShadowsFixedSpreadCutsceneDx9);
                schema.HandleFloat(ref SoftShadowsFixedSpreadCutsceneDx11);
            }

            schema.HandleFloat(ref SoftShadowsSpreadRatio);
            schema.HandleInt(ref WasSoftShadowsSampleCount);
            schema.HandleFloat(ref SoftShadowOverlap);
            if (Version > 0x2c)
            {
                schema.HandleFloat(ref SoftShadowCenterBias);
            }

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

            if (Version > 0x33)
            {
                schema.HandleByte(ref ShadowsOnDx11Only);
            }

            if (Version > 0x2e)
            {
                schema.HandleByte(ref DeferredBoundingBoxesAsExclusion);
                schema.HandleByte(ref UseBoundingBoxesForBlendedDirectional);
            }

            if (Version > 0x30)
            {
                schema.HandleFloat(ref StencilShadowExtrusionYPlane);
                schema.HandleFloat(ref StencilShadowExtrusionMaxLength);
            }

            if (Version > 0x32)
            {
                schema.HandleByte(ref UsesLightRestrictionBox);
            }
        }

        public NuMtx Mtx = new NuMtx();

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            Version = parentVersion;

            HandleLightDescCommon(schema);
            HandleLightDesc(schema);

            Mtx.Handle(schema, parentVersion);
        }
    }
}
