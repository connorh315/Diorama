using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Diorama.Rendering;

namespace Diorama;

public class TextureSlotControl : LabelledInput
{
    public static readonly StyledProperty<RenderTexture?> TextureProperty =
        AvaloniaProperty.Register<TextureSlotControl, RenderTexture?>(
            nameof(Texture));

    public RenderTexture? Texture
    {
        get => GetValue(TextureProperty);
        set => SetValue(TextureProperty, value);
    }

    public static readonly StyledProperty<IEnumerable<RenderTexture>> TexturesProperty =
        AvaloniaProperty.Register<TextureSlotControl, IEnumerable<RenderTexture>>(
            nameof(Textures));

    public IEnumerable<RenderTexture> Textures
    {
        get => GetValue(TexturesProperty);
        set => SetValue(TexturesProperty, value);
    }

    public static readonly StyledProperty<bool> IsPickerOpenProperty =
    AvaloniaProperty.Register<TextureSlotControl, bool>(
        nameof(IsPickerOpen));

public bool IsPickerOpen
{
    get => GetValue(IsPickerOpenProperty);
    set => SetValue(IsPickerOpenProperty, value);
}

    public static readonly StyledProperty<float?> ScaleProperty =
        AvaloniaProperty.Register<TextureSlotControl, float?>(
            nameof(Scale));

    public float? Scale
    {
        get => GetValue(ScaleProperty);
        set => SetValue(ScaleProperty, value);
    }

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
        AvaloniaProperty.Register<TextureSlotControl, int?>(
            nameof(SelectedSet));

    public int? SelectedSet
    {
        get => GetValue(SelectedSetProperty);
        set => SetValue(SelectedSetProperty, value);
    }

    public static readonly StyledProperty<object?> LayerBlendProperty =
        AvaloniaProperty.Register<TextureSlotControl, object?>(nameof(LayerBlend));

    public object? LayerBlend
    {
        get => GetValue(LayerBlendProperty);
        set
        {
            SetValue(LayerBlendProperty, value);
            OnPropertyChanged(nameof(LayerBlend));
        }
    }

    public bool HasLayerBlend => LayerBlend != null;

    private TexturePreviewControl mainPreview;
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (mainPreview != null)
            mainPreview.OnClick -= OnTextureButtonClick;

        mainPreview = (TexturePreviewControl)e.NameScope.Find("PART_MainTexture");

        if (mainPreview != null)
            mainPreview.OnClick += OnTextureButtonClick;
    }

    private void OnTextureButtonClick()
    {
        FlyoutBase.ShowAttachedFlyout(mainPreview);
    }
}