using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using OpenTK.Mathematics;

namespace Diorama;

public class TransformInput : LabelledInput
{
    private bool _updatingFromValue;

    private Vector3 _position;
    private Vector3 _eulerRotation;
    private Vector3 _scale = Vector3.One;

    private Quaternion _rotation = Quaternion.Identity;

    public static readonly StyledProperty<Matrix4> ValueProperty =
        AvaloniaProperty.Register<TransformInput, Matrix4>(
            nameof(Value),
            Matrix4.Identity,
            defaultBindingMode: BindingMode.TwoWay);

    public Matrix4 Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public static readonly DirectProperty<TransformInput, Vector3> PositionProperty =
        AvaloniaProperty.RegisterDirect<TransformInput, Vector3>(
            nameof(Position),
            o => o.Position,
            (o, v) => o.Position = v,
            defaultBindingMode: BindingMode.TwoWay);

    public Vector3 Position
    {
        get => _position;
        set
        {
            if (!SetAndRaise(PositionProperty, ref _position, value))
                return;

            if (!_updatingFromValue)
                TransformChanged();
        }
    }

    public static readonly DirectProperty<TransformInput, Vector3> RotationProperty =
        AvaloniaProperty.RegisterDirect<TransformInput, Vector3>(
            nameof(Rotation),
            o => o.Rotation,
            (o, v) => o.Rotation = v,
            defaultBindingMode: BindingMode.TwoWay);

    public Vector3 Rotation
    {
        get => _eulerRotation;
        set
        {
            if (!SetAndRaise(RotationProperty, ref _eulerRotation, value))
                return;

            _rotation = Quaternion.FromEulerAngles(_eulerRotation);

            if (!_updatingFromValue)
                TransformChanged();
        }
    }

    public static readonly DirectProperty<TransformInput, Vector3> ScaleProperty =
        AvaloniaProperty.RegisterDirect<TransformInput, Vector3>(
            nameof(Scale),
            o => o.Scale,
            (o, v) => o.Scale = v,
            defaultBindingMode: BindingMode.TwoWay);

    public Vector3 Scale
    {
        get => _scale;
        set
        {
            if (!SetAndRaise(ScaleProperty, ref _scale, value))
                return;

            if (!_updatingFromValue)
                TransformChanged();
        }
    }

    static TransformInput()
    {
        ValueProperty.Changed.AddClassHandler<TransformInput>(
            (control, _) => control.ValueChanged());
    }

    private void ValueChanged()
    {
        _updatingFromValue = true;

        try
        {
            var value = Value;

            Position = value.ExtractTranslation();

            _rotation = value.ExtractRotation();
            Rotation = _rotation.ToEulerAngles();

            Scale = value.ExtractScale();
        }
        finally
        {
            _updatingFromValue = false;
        }
    }

    private void TransformChanged()
    {
        var translation = Matrix4.CreateTranslation(Position);
        var scale = Matrix4.CreateScale(Scale);

        Matrix4.CreateFromQuaternion(in _rotation, out var rotation);

        Value = scale * rotation * translation;
    }
}