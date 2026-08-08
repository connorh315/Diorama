using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Diorama.Editor.Attributes;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;

namespace Diorama
{
    public class PropertyEditor : ContentControl
    {
        public static readonly StyledProperty<string> PropertyProperty =
            AvaloniaProperty.Register<PropertyEditor, string>(
                nameof(Property));

        public string Property
        {
            get => GetValue(PropertyProperty);
            set => SetValue(PropertyProperty, value);
        }

        public PropertyEditor()
        {
            DataContextChanged += (_, _) => Rebuild();
        }

        static PropertyEditor()
        {
            PropertyProperty.Changed.AddClassHandler<PropertyEditor>(
                (x, _) => x.Rebuild());
        }

        private Control BuildEditor(PropertyInfo property)
        {
            LabelledInput editor;

            if (property.PropertyType == typeof(bool))
            {
                editor = new CheckboxInput();
                editor.Bind(
                    CheckboxInput.ValueProperty,
                    new Binding(property.Name)
                    {
                        Mode = BindingMode.TwoWay
                    });
            }
            else if (property.PropertyType == typeof(float)
                     || property.PropertyType == typeof(string)
                     || property.PropertyType == typeof(byte)
                     || property.PropertyType == typeof(uint))
            {
                editor = new TextInput();
                editor.Bind(
                    Diorama.TextInput.ValueProperty,
                    new Binding(property.Name)
                    {
                        Mode = BindingMode.TwoWay
                    });
            }
            else if (property.PropertyType.IsEnum)
            {
                editor = new EnumInput()
                {
                    EnumType = property.PropertyType
                };

                editor.Bind(
                    EnumInput.SelectedValueProperty,
                    new Binding(property.Name)
                    {
                        Mode = BindingMode.TwoWay
                    });
            }
            else if (property.PropertyType == typeof(Vector4))
            {
                editor = new ColorInput();
                editor.Bind(
                    ColorInput.ColorProperty,
                    new Binding(property.Name)
                    {
                        Mode = BindingMode.TwoWay
                    });
            }
            else
            {
                throw new NotSupportedException();
            }

            return Configure(editor, property);
        }

        private LabelledInput Configure(
            LabelledInput editor,
            PropertyInfo property)
        {
            foreach (var attribute in property.GetCustomAttributes())
            {
                switch (attribute)
                {
                    case DisplayLabelAttribute display:
                        editor.InputLabel = display.Name;
                        break;

                    case EnabledIfAttribute enabled:
                        editor.Bind(
                            InputElement.IsEnabledProperty,
                            new Binding(enabled.Property));
                        break;

                    case VisibleIfAttribute visible:
                        editor.Bind(
                            Visual.IsVisibleProperty,
                            new Binding(visible.PropertyName));
                        break;

                    case IWarningAttribute warning:
                        editor.SetWarning(warning.WarningMessage);
                        break;
                }
            }


            //var display =
            //    property.GetCustomAttribute<DisplayLabelAttribute>();

            //if (display != null)
            //    editor.InputLabel = display.Name;

            //var enabled =
            //    property.GetCustomAttribute<EnabledIfAttribute>();

            //if (enabled != null)
            //{
            //    editor.Bind(
            //        InputElement.IsEnabledProperty,
            //        new Binding(enabled.Property));
            //}

            //var visible =
            //    property.GetCustomAttribute<VisibleIfAttribute>();

            //if (visible != null)
            //{
            //    editor.Bind(
            //        Visual.IsVisibleProperty,
            //        new Binding(visible.PropertyName));
            //}

            //var warnings = property.GetCustomAttributes<IWarningAttribute>();

            return editor;
        }

        private void Rebuild()
        {
            if (DataContext == null)
                return;

            var info = DataContext.GetType().GetProperty(Property);

            if (info == null)
                return;

            Content = BuildEditor(info);
        }
    }
}
