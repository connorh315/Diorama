using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama
{
    public class DragLabel : TextBlock
    {
        private Point _start;
        private bool _dragging;

        public static readonly StyledProperty<float> ValueProperty =
            AvaloniaProperty.Register<DragLabel, float>(nameof(Value));

        public float Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public float Sensitivity { get; set; } = 0.1f;

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);

            _dragging = true;
            _start = e.GetPosition(this);

            e.Pointer.Capture(this);
        }

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            base.OnPointerMoved(e);

            if (!_dragging)
                return;

            var current = e.GetPosition(this);
            var deltaX = current.X - _start.X;

            Value += (float)deltaX * Sensitivity;

            _start = current; // accumulate movement
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);

            _dragging = false;
            e.Pointer.Capture(null);
        }
    }
}
