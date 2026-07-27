using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Diorama.UI.Progress;
using Diorama.UI.ViewModels;
using Diorama.UI.Windows;

namespace Diorama;

public partial class SettingsWindow : ModalWindow
{
    public bool CleanExit = false;

    public SettingsWindow() : base("Settings")
    {
        InitializeComponent();

        DataContext = AppSettings.Settings;

        this.Closing += (s, e) =>
        {
            if (!CleanExit)
            { // If the user tries to just close the window via the "X" then rebuild the app settings
                AppSettings.Settings = AppSettings.Load();
            }
        };
    }

    private void SaveSettings_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        AppSettings.Settings.Save();
        CleanExit = true;
        Close();
    }

    private void GenerateFingerprints_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var cacheProgressWindow = new GenerateFingerprintsWindow();
        cacheProgressWindow.ShowDialog(this);
    }
}