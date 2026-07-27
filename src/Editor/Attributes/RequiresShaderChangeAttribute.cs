using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.Editor.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class RequiresShaderChangeAttribute : Attribute, IWarningAttribute
    {
        public string WarningMessage { get => "Requires shader change"; }

        public RequiresShaderChangeAttribute()
        {

        }
    }
}
