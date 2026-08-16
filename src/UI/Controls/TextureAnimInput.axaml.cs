using Avalonia;
using Avalonia.Controls.Primitives;
using Diorama.Editor.Material;

namespace Diorama;

public class TextureAnimInput : TemplatedControl
{
    public static readonly StyledProperty<EditorMaterialTextureAnim?> AnimProperty =
        AvaloniaProperty.Register<TextureAnimInput, EditorMaterialTextureAnim?>(
            nameof(Anim));

    public EditorMaterialTextureAnim? Anim
    {
        get => GetValue(AnimProperty);
        set => SetValue(AnimProperty, value);
    }

}