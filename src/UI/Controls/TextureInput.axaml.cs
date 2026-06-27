using Avalonia;
using Avalonia.Controls.Primitives;
using Diorama.Rendering;

namespace Diorama;

public class TextureInput : LabelledInput
{
    public static readonly StyledProperty<RenderTexture?> TextureProperty =
        AvaloniaProperty.Register<TextureInput, RenderTexture?>(
            nameof(Texture));

    public RenderTexture? Texture
    {
        get => GetValue(TextureProperty);
        set => SetValue(TextureProperty, value);
    }
}