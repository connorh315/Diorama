using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Diorama.Rendering;
using Diorama.UI.Panels;

namespace Diorama;

public partial class MaterialPanel : UserControl, ITexturesPanel
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