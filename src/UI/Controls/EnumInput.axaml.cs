using Avalonia;
using Avalonia.Controls.Primitives;
using System.Collections;

namespace Diorama;

public class EnumInput : LabelledInput
{
    public static readonly StyledProperty<Type?> EnumTypeProperty =
    AvaloniaProperty.Register<EnumInput, Type?>(nameof(EnumType));

    public Type? EnumType
    {
        get => GetValue(EnumTypeProperty);
        set => SetValue(EnumTypeProperty, value);
    }

    public static readonly DirectProperty<EnumInput, IEnumerable> ValuesProperty =
    AvaloniaProperty.RegisterDirect<EnumInput, IEnumerable>(
        nameof(Values),
        o => o.Values,
        (o, v) => o.Values = v);

    private IEnumerable _values = Array.Empty<object>();

    public IEnumerable Values
    {
        get => _values;
        private set => SetAndRaise(ValuesProperty, ref _values, value);
    }

    public static readonly StyledProperty<object?> SelectedValueProperty =
        AvaloniaProperty.Register<EnumInput, object?>(nameof(SelectedValue));

    public object? SelectedValue
    {
        get => GetValue(SelectedValueProperty);
        set => SetValue(SelectedValueProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == EnumTypeProperty)
        {
            Values = EnumType is null
                ? Array.Empty<object>()
                : Enum.GetValues(EnumType);
        }
    }
}