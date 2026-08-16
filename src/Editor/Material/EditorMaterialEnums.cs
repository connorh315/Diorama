using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.Editor.Material
{
    public enum EditorBlendMode
    {
        Off = 0,
        Blended,
        Additive,
        Subtract,
        PreMultipliedAlpha,
        ScreenDoorOld,
        Scale,
        ModTxt,
        MaxColour,
        Multiply,
        AddRGBA,
        AddRGBSrcAlpha,
        AddRGBDstAlpha,
        PreMultipliedSubtract,
        MultiplyAdd,
        InverseMultiply,
        BlendedDstAlpha
    }

    public enum EditorDiffuseBlendMode
    {
        Disabled = -1,
        Off,
        Over,
        Add,
        Subtract,
        Multiply,
        MaxAlpha,
        AddMask,
        Scale,
        MayaWiiMaxAlphaPreview,
        MaskWithLowerLayerX,
        MaxAlphaBlend,
        MultiplyRed,
        MultiplyGreen,
        MultiplyBlue,
        AddRed,
        AddGreen,
        AddBlue,
        SubtractRed,
        SubtractGreen,
        SubtractBlue,
        ThresholdLayer1Alpha,
        OverRGB,
        BackFace,
        Parallax,
        Layer1GreyscaleRed,
        Layer1GreyscaleGreen,
        Layer1GreyscaleBlue,
        Layer1Unlit,
        AddGreyscaleFromRed,
        AddGreyscaleFromGreen,
        AddGreyscaleFromBlue
    }

    public enum EditorNormalBlendMode
    {
        Disabled = 0,
        Over,
        Detail,
        RoughDetail,
        BackFace
    }

    public enum EditorSpecularBlendMode
    {
        Disabled = 0,
        Over,
        MulAOAddRoughness,
        BackFace
    }

    public enum EditorAlphaTestMode
    {
        NeverDraw = 0,
        Always,
        Less,
        LessEqual,
        Equal,
        GreaterEqual,
        Greater,
        NotEqual,
        CheapSort
    }

    public enum EditorRefraction
    {
        Disabled = 0,
        FixedDepth,
        DynamicDepth,
        DefunctWiiWater,
        DefunctWiiGlass
    }

    public enum EditorReflectionMode
    {
        Disabled = 0,
        CustomEnvironmentMap,
        BakedEnvironmentMap,
        LightEnvironmentMap,
        ScaledBakedEnvironmentMap,
        PBREnvironmentMap
    }

    public enum EditorBakedLightingMode
    {
        None = 0,
        FlatLit,
        TextureLit,
        VertexLit,
        QuadraticSH,
        DefunctViewSpaceShaded,
        DefunctTextureNXGVertWii
    }

    public enum EditorRoughnessMode
    {
        FullySmooth = 0,
        FullyRough,
        DefunctConstantLowRoughness,
        DefunctConstantHighRoughness,
        DefunctVaryingLowRoughness,
        DefunctVaryingHighRoughness,
        DefunctFullRange,
        Constant,
        Varying
    }

    public enum EditorSurfaceMapFormat
    {
        xyzh = 0,
        hyzx,
        xyZh,
        hyZx,
        Ry_x,
        RyZx,
        RdYSdX
    }

    public enum EditorSubstanceMode
    {
        NonMetal = 0,
        Constant,
        Varying,
        Metal
    }

    public enum EditorShaderType
    {
        Custom = 0,
        Water,
        LegoHard,
        LegoMetallic,
        LegoChrome,
        LegoCanvas,
        LegoFluorescent,
        LegoGlitter,
        LegoHands,
        LegoHardRubber,
        LegoSoftRubber,
        LegoPearlescent,
        LegoPlants,
        LegoSoft,
        LegoStippled,
        LegoTransparent,
        LegoSuperChar,
        LegoGamePlay,
        LegoGlow,
        LegoIce,
        LegoUnused4,
        LegoUnused5
    }

    public enum EditorLightingModel
    {
        NonReflective = 0,
        Lambert,
        BlinnPhong,
        Anisotropic,
        DefunctSkin,
        DefunctComplexSkin,
        DefunctHair
    }
}
