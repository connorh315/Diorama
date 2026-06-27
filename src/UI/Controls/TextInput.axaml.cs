using Avalonia;
using Avalonia.Controls.Primitives;
using Diorama.UI.Controls;

namespace Diorama;

public class TextInput : LabelledInput
{
    public static readonly StyledProperty<string> ValueProperty =
        AvaloniaProperty.Register<TextInput, string>(nameof(Value));

    public string? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }
}