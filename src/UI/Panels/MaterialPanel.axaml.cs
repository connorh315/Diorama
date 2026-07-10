using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Diorama.Rendering;

namespace Diorama;

public partial class MaterialPanel : UserControl
{
    public MaterialPanel()
    {
        InitializeComponent();
    }

    public static readonly StyledProperty<IEnumerable<RenderTexture>> TexturesProperty =
        AvaloniaProperty.Register<MaterialPanel, IEnumerable<RenderTexture>>(
            nameof(Textures));

    public IEnumerable<RenderTexture> Textures
    {
        get => GetValue(TexturesProperty);
        set => SetValue(TexturesProperty, value);
    }
}