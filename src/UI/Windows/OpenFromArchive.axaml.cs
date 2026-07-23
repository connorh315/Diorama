using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Diorama.UI.ViewModels;
using Diorama.UI.Windows;
using static System.Net.WebRequestMethods;

namespace Diorama;

public partial class OpenFromArchive : ModalWindow
{
    public OpenFromArchive() : base("Open From Archives")
    {
        InitializeComponent();
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);

        SearchBox.Focus();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.Key == Key.Enter)
        {
            Open();
        }
    }

    public void Open()
    {
        if (DataContext is OpenFromArchiveViewModel vm && vm.OpenSelected())
        {
            Close();
        }
    }

    private void FilesList_DoubleTapped(object? sender, TappedEventArgs e)
    {
        Open();
    }
}