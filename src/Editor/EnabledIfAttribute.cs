using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.Editor
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DisplayAttribute : Attribute
    {
        public string Name { get; }

        public DisplayAttribute(string name)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class EnabledIfAttribute : Attribute
    {
        public string Property { get; }

        public EnabledIfAttribute(string property)
        {
            Property = property;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class VisibleIfAttribute : Attribute
    {
        public string PropertyName { get; }

        public VisibleIfAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}
