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
            else if (property.PropertyType == typeof(float))
            {
                editor = new FloatInput();
                editor.Bind(
                    Diorama.TextInput.ValueProperty,
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
                var binding = new Binding(property.Name)
                {
                    Mode = BindingMode.TwoWay
                };
                editor.Bind(Diorama.TextInput.ValueProperty, binding);
                ((TextInput)editor).ValueBinding = binding;
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
            else if (property.PropertyType == typeof(Matrix4))
            {
                editor = new TransformInput();
                editor.Bind(
                    TransformInput.ValueProperty,
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

                    case SliderAttribute slider:
                        FloatInput floatInput = (FloatInput)editor;
                        floatInput.Minimum = slider.LowestValue;
                        floatInput.Maximum = slider.HighestValue;
                        floatInput.ShowSlider = true;
                        break;

                    case FSFolderAttribute fs:
                        FSInput newEditor = new FSInput();
                        newEditor.Bind(Diorama.FSInput.ValueProperty, ((TextInput)editor).ValueBinding);
                        newEditor.InputLabel = editor.InputLabel;
                        editor = newEditor;
                        break;

                    case IWarningAttribute warning:
                        editor.SetWarning(warning.WarningMessage);
                        break;
                }
            }

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
