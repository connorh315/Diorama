using Diorama.Editor.Attributes;
using Diorama.Editor.Material;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;

namespace Diorama.Editor.ShaderSystem
{
    public class EditorShaderComparison : EditableItem
    {
        public string PropertyName { get; set; }

        public string MaterialValue { get; set; }

        public string SelectedValue { get; set; }
        
        public EditorShaderComparison(string propertyName, string value)
        {
            PropertyInfo property = typeof(EditorMaterial).GetProperty(propertyName);
            bool set = false;
            foreach (var attribute in property.GetCustomAttributes())
            {
                switch (attribute)
                {
                    case DisplayLabelAttribute display:
                        PropertyName = display.Name;
                        set = true;
                        break;
                }

                if (set)
                    break;
            }

            if (!set)
                PropertyName = propertyName;

            MaterialValue = value;
        }

        public void UpdateSelected(string selectedValue)
        {
            SelectedValue = selectedValue;
            OnPropertyChanged(nameof(SelectedValue));
        }
    }
}
