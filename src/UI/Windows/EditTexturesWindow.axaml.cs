using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Diorama.Core.Filetypes.GSC.Components;
using Diorama.Core.Filetypes.TEXTURES;
using Diorama.Rendering;
using Diorama.UI.ViewModels;

namespace Diorama;

public partial class EditTexturesWindow : Window
{
    public EditTexturesWindow()
    {
        InitializeComponent();
    }

    public EditTexturesWindow(EditTexturesViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        PART_MainTexture.OnClick += OnTextureButtonClick;
        AddNewTexture.Click += AddNewTexture_Click;
        RemoveTexture.Click += RemoveTexture_Click;
    }

    private async void OnTextureButtonClick()
    {
        if (StorageProvider == null)
            throw new Exception("Unable to access filesystem");

        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Replace DDS Image",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("DDS files") { Patterns = new[] { "*.DDS" } }
            }
        });

        if (files.Count > 0 && DataContext is EditTexturesViewModel vm)
        {
            string filePath = files[0].Path.LocalPath;

            NuTexture texture = NuTexture.Load(filePath, vm.Texture.Original.Header);

            var tex = vm.Texture;

            int slot = TexturePicker.GetSlot(vm.Texture);

            TexturePicker.SetSlot(slot, RenderTexture.GetWhiteTexture()); // trigger re-draw

            RenderService.Current.Enqueue(() =>
            {
                tex.Reload(texture);
                TexturePicker.SetSlot(slot, vm.Texture);
                PART_MainTexture.Reload();
                //vm.Texture = tex;
                //TexturePicker.RefreshTexture(vm.Texture);
            });
        }
    }

    private void AddNewTexture_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is not EditTexturesViewModel vm)
            return;

        RenderService.Current.Enqueue(() =>
        {
            NuTexGenHdr texHdr = new NuTexGenHdr();
            texHdr.RandomiseChecksum();
            NuTexture tex = new NuTexture()
            {
                Header = texHdr
            };

            RenderTexture newTexture = RenderTexture.FromNuTexture(tex);

            Dispatcher.UIThread.Post(() =>
            {
                vm.Textures.Add(newTexture);
                vm.Texture = newTexture;
                TexturePicker.ApplyFilter();
                TexturePicker.MakeVisible(vm.Textures.Count - 1);
            });
        });
    }

    private void RemoveTexture_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is not EditTexturesViewModel vm || vm.Texture == null)
            return;

        var tex = vm.Texture;

        int index = vm.Textures.IndexOf(tex);

        vm.Textures.Remove(tex);

        vm.Texture = null;

        TexturePicker.ApplyFilter();

        TexturePicker.MakeVisible(index);

        tex.Delete();
    }
}