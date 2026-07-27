using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Diorama.Editor
{
    public class EditableItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        protected bool Set<T>(
            ref T field,
            T value,
            [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

#if DEBUG
        protected bool GetBoolByte(byte value)
        {
            if (value == 1) return true;
            if (value == 0) return false;
            throw new InvalidOperationException($"Invalid byte value for boolean conversion: {value}");
        }
#else
        protected bool GetBoolByte(byte value) => value != 0;
#endif

        protected bool SetBoolByte(ref byte field, bool value, [CallerMemberName] string? propertyName = null)
        {
            byte newValue = value ? (byte)1 : (byte)0;

            if (field == newValue)
                return false;

            field = newValue;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected float GetFloatByte(byte value)
        {
            return value / 255f;
        }

        protected bool SetFloatByte(ref byte field, float value, [CallerMemberName] string? propertyName = null)
        {
            byte newValue = (byte)Math.Round(value * 255);

            if (field == newValue)
                return false;

            field = newValue;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected int ConvertColour(Vector4 col)
        {
            uint r = (uint)(Math.Clamp(col.X, 0f, 1f) * 255f);
            uint g = (uint)(Math.Clamp(col.Y, 0f, 1f) * 255f);
            uint b = (uint)(Math.Clamp(col.Z, 0f, 1f) * 255f);
            uint a = (uint)(Math.Clamp(col.W, 0f, 1f) * 255f);

            return (int)(
                (a << 24) |
                (b << 16) |
                (g << 8) |
                (r << 0));
        }

        protected Vector4 GetColour(int col)
        {
            uint abgr = (uint)col;
            float a = ((abgr >> 24) & 0xFF) / 255f;
            float b = ((abgr >> 16) & 0xFF) / 255f;
            float g = ((abgr >> 8) & 0xFF) / 255f;
            float r = ((abgr >> 0) & 0xFF) / 255f;
            return new Vector4(r, g, b, a);
        }

        protected bool SetColour(ref int field, Vector4 value, [CallerMemberName] string? propertyName = null)
        {
            int newValue = ConvertColour(value);

            if (field == newValue)
                return false;

            field = newValue;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
