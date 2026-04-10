using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Diorama.Editor.Metadata;
using Diorama.UI.ViewModels;

namespace Diorama;

public partial class EditResourceHeaderWindow : Window
{
    public EditResourceHeaderWindow()
    {
        InitializeComponent();
    }

    public EditResourceHeaderWindow(ResourceHeaderViewModel viewmodel)
    {
        InitializeComponent();
        DataContext = viewmodel;
        Width = 700;
        SizeToContent = SizeToContent.Height;

        Title = "Edit Resource Header entries";
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