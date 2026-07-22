using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Diorama.Rendering;
using Diorama.UI.Panels;

namespace Diorama;

public partial class LightmapPanel : UserControl, ITexturesPanel
{
    public LightmapPanel()
    {
        InitializeComponent();
    }

    public static readonly StyledProperty<IEnumerable<RenderTexture>> TexturesProperty =
        AvaloniaProperty.Register<LightmapPanel, IEnumerable<RenderTexture>>(
            nameof(Textures));

    public IEnumerable<RenderTexture> Textures
    {
        get => GetValue(TexturesProperty);
        set => SetValue(TexturesProperty, value);
    }
}