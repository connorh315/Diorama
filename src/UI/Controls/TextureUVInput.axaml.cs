using Avalonia;
using Avalonia.Controls.Primitives;
using Diorama.Rendering;

namespace Diorama;

public class TextureUVInput : TextureInput
{
    public IReadOnlyList<UVSetOption> UVSets { get; } = 
    [
        new() { Value = -1, Name = "Disabled" },
        new() { Value = 0, Name = "UV Set 1" },
        new() { Value = 1, Name = "UV Set 2" },
        new() { Value = 2, Name = "UV Set 3" },
        new() { Value = 3, Name = "UV Set 4" },
    ];

    public UVSetOption? SelectedUVSet
    {
        get => UVSets.FirstOrDefault(x => x.Value == SelectedSet);
        set => SelectedSet = value?.Value;
    }

    public static readonly StyledProperty<int?> SelectedSetProperty =
        AvaloniaProperty.Register<TextureUVInput, int?>(
            nameof(SelectedSet));

    public int? SelectedSet
    {
        get => GetValue(SelectedSetProperty);
        set => SetValue(SelectedSetProperty, value);
    }

    public static readonly StyledProperty<float?> ScaleProperty =
        AvaloniaProperty.Register<TextureUVInput, float?>(
            nameof(Scale));

    public float? Scale
    {
        get => GetValue(ScaleProperty);
        set => SetValue(ScaleProperty, value);
    }
}

public class UVSetOption
{
    public int Value { get; init; }
    public string Name { get; init; } = string.Empty;
}