using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.Editor.Attributes
{
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
