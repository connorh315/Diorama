using Avalonia;
using Avalonia.Input;
using Diorama.Rendering;
using Diorama.UI.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama
{
    public class TexturePreviewControl : GlHost
    {
        public static readonly StyledProperty<RenderTexture?> TextureProperty =
            AvaloniaProperty.Register<TexturePreviewControl, RenderTexture?>(
                nameof(Texture));

        public RenderTexture? Texture
        {
            get => GetValue(TextureProperty);
            set => SetValue(TextureProperty, value);
        }

        //public static readonly StyledProperty<Cursor?> CursorProperty =
        //    AvaloniaProperty.Register<TexturePreviewControl, Cursor?>(
        //        nameof(Cursor));

        //public Cursor? Cursor
        //{
        //    get => GetValue(CursorProperty);
        //    set => SetValue(CursorProperty, value);
        //}

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == TextureProperty && renderer is TextureRenderer texRenderer)
            {
                texRenderer.SetActiveTexture((RenderTexture?)change.NewValue);
                surface?.IsDirty = true;
            }
        }

        public Action? OnClick;

        protected override void OnReleaseLeftClick()
        {
            OnClick?.Invoke();
        }

        public void Reload()
        {
            surface.IsDirty = true;
        }

        public TexturePreviewControl() : base(new TextureRenderer())
        {
            
        }
    }
}
