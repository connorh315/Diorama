using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.Editor
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
}
