using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Diorama.Editor;
using Diorama.UI.Controls;
using System.Collections.ObjectModel;

namespace Diorama;

public class ListInput : LabelledInput
{
    public static readonly StyledProperty<IEnumerable<INamedItem>> ItemsProperty =
    AvaloniaProperty.Register<ListInput, IEnumerable<INamedItem>>(nameof(Items));

    public IEnumerable<INamedItem>? Items
    {
        get => GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    public static readonly StyledProperty<INamedItem?> SelectedItemProperty =
        AvaloniaProperty.Register<ListInput, INamedItem?>(
            nameof(SelectedItem),
            defaultBindingMode: BindingMode.TwoWay);

    public INamedItem? SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }
}