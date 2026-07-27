using Avalonia;
using Avalonia.Controls;
using Diorama.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.UI.Panels
{
    public class TexturesBasePanel : UserControl
    {
        public static readonly StyledProperty<IEnumerable<RenderTexture>> TexturesProperty =
        AvaloniaProperty.Register<TexturesBasePanel, IEnumerable<RenderTexture>>(
            nameof(Textures));

        public IEnumerable<RenderTexture> Textures
        {
            get => GetValue(TexturesProperty);
            set => SetValue(TexturesProperty, value);
        }
    }
}
