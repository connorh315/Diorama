using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Diorama.Editor.Metadata;
using Diorama.UI.ViewModels;
using Diorama.UI.Windows;

namespace Diorama;

public partial class EditResourceHeaderWindow : ModalWindow
{
    public EditResourceHeaderWindow() : base("Edit Resource Header entries")
    {
        InitializeComponent();
    }

    public EditResourceHeaderWindow(ResourceHeaderViewModel viewmodel) : this()
    {
        DataContext = viewmodel;
        Width = 700;
        SizeToContent = SizeToContent.Height;
    }

    private void AddNewEntry_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is not ResourceHeaderViewModel vm)
            return;

        vm.References.Add(new EditorResourceReference());
    }

    private void RemoveEntry_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Console.WriteLine(sender);
    }
}