using Avalonia;
using Avalonia.Controls.Primitives;

namespace Diorama;

public class FloatInput : TextInput
{
    public static readonly StyledProperty<float> MinimumProperty =
        AvaloniaProperty.Register<FloatInput, float>(nameof(Minimum));

    public float? Minimum
    {
        get => GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public static readonly StyledProperty<float> MaximumProperty =
    AvaloniaProperty.Register<FloatInput, float>(nameof(Maximum));

    public float? Maximum
    {
        get => GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public float StepAmount { get; set; }

    public bool ShowSlider { get; set; }
}