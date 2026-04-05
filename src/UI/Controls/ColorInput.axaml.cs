using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Media;
using OpenTK.Mathematics;

namespace Diorama;

public class ColorInput : LabelledInput
{
    public ColorInput()
    {
        ColorProperty.Changed.AddClassHandler<ColorInput>((x, e) =>
        {
            x.RaisePropertyChanged(
            UIColorProperty,
            default,
            default);
        });
        //ValueProperty.Changed.AddClassHandler<Vec3Input>((x, e) =>
        //{
        //    x.RaisePropertyChanged(XProperty, default, x.X);
        //    x.RaisePropertyChanged(YProperty, default, x.Y);
        //    x.RaisePropertyChanged(ZProperty, default, x.Z);
        //});
    }


    /// <summary>
    /// Color StyledProperty definition
    /// </summary>
    public static readonly StyledProperty<Vector4> ColorProperty =
        AvaloniaProperty.Register<ColorInput, Vector4>(nameof(Color));

    /// <summary>
    /// Gets or sets the Color property. This StyledProperty
    /// indicates ....
    /// </summary>
    public Vector4 Color
    {
        get => this.GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    public static readonly DirectProperty<ColorInput, Color> UIColorProperty =
    AvaloniaProperty.RegisterDirect<ColorInput, Color>(
        nameof(UIColor),
        o => o.UIColor,
        (o, v) => o.UIColor = v);

    public Color UIColor
    {
        get => new Color((byte)(Color.W * 255), (byte)(Color.X * 255), (byte)(Color.Y * 255), (byte)(Color.Z * 255));
        set => Color = new Vector4(value.R / 255f, value.G / 255f, value.B / 255f, value.A / 255f);
    }

    ///// <summary>
    ///// Value StyledProperty definition
    ///// </summary>
    //public static readonly StyledProperty<Vector3> ValueProperty =
    //    AvaloniaProperty.Register<Vec3Input, Vector3>(nameof(Value));

    ///// <summary>
    ///// Gets or sets the Value property. This StyledProperty 
    ///// indicates ....
    ///// </summary>
    //public Vector3 Value
    //{
    //    get => this.GetValue(ValueProperty);
    //    set
    //    {
    //        SetValue(ValueProperty, value);
    //    }
    //}

    //private bool IsHalf = false;

    //private bool IsValid(float val)
    //{
    //    return true;
    //}

    //public static readonly DirectProperty<Vec3Input, float> XProperty =
    //AvaloniaProperty.RegisterDirect<Vec3Input, float>(
    //    nameof(X),
    //    o => o.X,
    //    (o, v) => o.X = v,
    //    defaultBindingMode: BindingMode.TwoWay);

    //private float x;
    //public float X
    //{
    //    get => Value.X;
    //    set
    //    {
    //        if (Value.X == value)
    //            return;

    //        var v = Value;
    //        v.X = value;
    //        Value = v;
    //    }
    //}

    //public static readonly DirectProperty<Vec3Input, float> YProperty =
    //AvaloniaProperty.RegisterDirect<Vec3Input, float>(
    //    nameof(Y),
    //    o => o.Y,
    //    (o, v) => o.Y = v,
    //    defaultBindingMode: BindingMode.TwoWay);

    //private float y;
    //public float Y
    //{
    //    get => Value.Y;
    //    set
    //    {
    //        if (Value.Y == value)
    //            return;

    //        var v = Value;
    //        v.Y = value;
    //        Value = v;
    //    }
    //}

    //public static readonly DirectProperty<Vec3Input, float> ZProperty =
    //    AvaloniaProperty.RegisterDirect<Vec3Input, float>(
    //        nameof(Z),
    //        o => o.Z,
    //        (o, v) => o.Z = v,
    //        defaultBindingMode: BindingMode.TwoWay);

    //private float z;
    //public float Z
    //{
    //    get => Value.Z;
    //    set
    //    {
    //        if (Value.Z == value)
    //            return;

    //        var v = Value;
    //        v.Z = value;
    //        Value = v;
    //    }
    //}


    //private void TextBox1_LostFocus(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    //{
    //    if (sender is not TextBox textbox) return;
    //}

    //private void TextBox2_LostFocus(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    //{
    //    if (sender is not TextBox textbox) return;
    //}

    //private void TextBox3_LostFocus(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    //{
    //    if (sender is not TextBox textbox) return;
    //}

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        //var tb1 = e.NameScope.Find<TextBox>("PART_Textbox1");
        //tb1.LostFocus += TextBox1_LostFocus;

        //var tb2 = e.NameScope.Find<TextBox>("PART_Textbox2");
        //tb2.LostFocus += TextBox2_LostFocus;

        //var tb3 = e.NameScope.Find<TextBox>("PART_Textbox3");
        //tb3.LostFocus += TextBox3_LostFocus;
    }
}