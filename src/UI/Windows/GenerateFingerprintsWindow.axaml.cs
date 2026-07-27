using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Diorama.UI.ViewModels;
using Diorama.UI.Windows;

namespace Diorama;

public partial class GenerateFingerprintsWindow : ModalWindow
{
    private GenerateFingerprintCacheViewModel viewmodel;

    public GenerateFingerprintsWindow() : base("Generating Fingerprints")
    {
        InitializeComponent();

        viewmodel = new GenerateFingerprintCacheViewModel(this);
        DataContext = viewmodel;

        Loaded += async (_, _) =>
        {
            await viewmodel.RunAsync();
        };

        Closing += async (_, _) =>
        {
            viewmodel.Cancel();
        };
    }
}