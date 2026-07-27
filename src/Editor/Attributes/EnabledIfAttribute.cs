using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.Editor.Attributes
{

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class EnabledIfAttribute : Attribute
    {
        public string Property { get; }

        public EnabledIfAttribute(string property)
        {
            Property = property;
        }
    }
}
