using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Diorama.Rendering;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Diorama;

public partial class TexturePicker : UserControl
{
    public static readonly StyledProperty<RenderTexture?> SelectedTextureProperty =
            AvaloniaProperty.Register<TexturePicker, RenderTexture?>(
                nameof(SelectedTexture));

    public RenderTexture? SelectedTexture
    {
        get => GetValue(SelectedTextureProperty);
        set => SetValue(SelectedTextureProperty, value);
    }

    public static readonly StyledProperty<string> SearchTextProperty =
        AvaloniaProperty.Register<TexturePicker, string>(
            nameof(SearchText), "");

    public string SearchText
    {
        get => GetValue(SearchTextProperty);
        set => SetValue(SearchTextProperty, value);
    }

    public static readonly StyledProperty<IEnumerable<RenderTexture>> ItemsSourceProperty =
        AvaloniaProperty.Register<TexturePicker, IEnumerable<RenderTexture>>(
            nameof(ItemsSource));

    public IEnumerable<RenderTexture> ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public ObservableCollection<RenderTexture> FilteredTextures { get; } = new();

    public List<TextureSlot> VisibleTextures { get; } = new();

    public TexturePicker()
    {
        InitializeComponent();

        for (int i = 0; i < 20; i++)
            VisibleTextures.Add(new TextureSlot());

        var prev = this.FindControl<Button>("PART_Previous");

        if (prev != null)
            prev.Click += (e, sender) => { UpdateIndex(-20); };

        var next = this.FindControl<Button>("PART_Next");
        if (next != null)
            next.Click += (e, sender) => { UpdateIndex(20); };
    }

    int firstVisibleIndex = 0;

    private void UpdateVisibleTextures()
    {
        for (int i = 0; i < VisibleTextures.Count; i++)
        {
            int index = firstVisibleIndex + i;

            VisibleTextures[i].Texture =
                index < FilteredTextures.Count
                    ? FilteredTextures[index]
                    : null;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == ItemsSourceProperty)
        {
            ApplyFilter();
        }

        if (change.Property == SearchTextProperty)
        {
            ApplyFilter();
        }
    }

    private void ApplyFilter()
    {
        FilteredTextures.Clear();

        if (ItemsSource == null)
            return;

        foreach (var texture in ItemsSource)
        {
            if (string.IsNullOrWhiteSpace(SearchText) ||
                texture.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
            {
                FilteredTextures.Add(texture);
            }
        }

        firstVisibleIndex = 0;
        UpdateVisibleTextures();
    }

    private void UpdateIndex(int change)
    {
        firstVisibleIndex = Math.Clamp(firstVisibleIndex + change, 0, ((FilteredTextures.Count - 1) / 20) * 20);

        UpdateVisibleTextures();
    }

    private void TexturePreview_Attached(object? sender, VisualTreeAttachmentEventArgs e)
    {
        var preview = (TexturePreviewControl)sender!;
        preview.OnClick += () =>
        {
            SelectedTexture = preview.Texture;
        };
    }
}

public class TextureSlot : INotifyPropertyChanged
{
    private RenderTexture? _texture;

    public RenderTexture? Texture
    {
        get => _texture;
        set
        {
            if (_texture == value)
                return;

            _texture = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Texture)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}