using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Interactivity;
using Diorama.Editor;
using Diorama.UI.Controls;
using System.Collections.ObjectModel;
using System.Windows.Input;

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

    public static readonly StyledProperty<bool> CanBeNullProperty =
        AvaloniaProperty.Register<ListInput, bool>(nameof(CanBeNull), true);

    public bool CanBeNull
    {
        get => GetValue(CanBeNullProperty);
        set => SetValue(CanBeNullProperty, value);
    }

    public static readonly StyledProperty<object?> SelectedItemProperty =
        AvaloniaProperty.Register<ListInput, object?>(nameof(SelectedItem));

    public object? SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public static readonly StyledProperty<ICommand?> OnSelectionChangedProperty =
        AvaloniaProperty.Register<ListInput, ICommand?>(
            nameof(OnSelectionChanged),
            defaultBindingMode: BindingMode.TwoWay);

    public ICommand OnSelectionChanged
    {
        get => GetValue(OnSelectionChangedProperty);
        set => SetValue(OnSelectionChangedProperty, value);
    }

    private Button? _button;
    private Popup? _popup;
    private ListBox? _list;
    private Button? _unlink;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (_button is not null)
            _button.Click -= ButtonClicked;

        if (_list is not null)
            _list.SelectionChanged -= SelectionChanged;

        _button = e.NameScope.Find<Button>("PART_Button");
        _popup = e.NameScope.Find<Popup>("PART_Popup");
        _list = e.NameScope.Find<ListBox>("PART_List");
        _unlink = e.NameScope.Find<Button>("PART_Unlink");

        if (_button is not null)
            _button.Click += ButtonClicked;

        if (_list is not null)
            _list.SelectionChanged += SelectionChanged;

        if (_unlink is not null)
            _unlink.Click += UnlinkClicked;
    }

    private void UnlinkClicked(object? sender, RoutedEventArgs e)
    {
        SelectedItem = null;
    }

    private void ButtonClicked(object? sender, RoutedEventArgs e)
    {
        if (_popup is null || _list is null)
            return;

        _list.SelectedItem = SelectedItem;
        _popup.IsOpen = true;
    }

    private void SelectionChanged(
        object? sender,
        SelectionChangedEventArgs e)
    {
        if (_list?.SelectedItem is not INamedItem item)
            return;

        SelectedItem = item;

        if (_popup is not null)
            _popup.IsOpen = false;
    }
}