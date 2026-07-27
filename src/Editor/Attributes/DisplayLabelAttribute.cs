using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.Editor.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DisplayLabelAttribute : Attribute
    {
        public string Name { get; }

        public DisplayLabelAttribute(string name)
        {
            Name = name;
        }
    }
}
