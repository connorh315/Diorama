using Avalonia;
using Avalonia.Controls.Primitives;
using Diorama.Rendering;

namespace Diorama;

public class CheckboxInput : LabelledInput
{
    public static readonly StyledProperty<bool?> ValueProperty =
        AvaloniaProperty.Register<CheckboxInput, bool?>(
            nameof(Value));

    public bool? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }
}