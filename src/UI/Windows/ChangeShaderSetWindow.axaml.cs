using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Diorama.UI.ViewModels;
using Diorama.UI.Windows;

namespace Diorama;

public partial class ChangeShaderSetWindow : ModalWindow
{
    public ChangeShaderSetWindow() : base("Change shader set")
    {
        InitializeComponent();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.Key == Key.Enter)
        {
            CommitChange();
        }
    }

    public void CommitChange()
    {
        if (DataContext is ChangeShaderSetViewModel vm)
        {
            vm.ChangeFingerprint();
            Close();
        }
    }

    private void FingerprintList_DoubleTapped(object? sender, TappedEventArgs e)
    {
        CommitChange();
    }
}