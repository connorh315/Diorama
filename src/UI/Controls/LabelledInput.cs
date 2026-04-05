using Avalonia;
using Avalonia.Controls.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama
{
    public class LabelledInput : TemplatedControl
    {
        /// <summary>
        /// InputLabel StyledProperty definition
        /// </summary>
        public static readonly StyledProperty<string> InputLabelProperty =
            AvaloniaProperty.Register<LabelledInput, string>(nameof(InputLabel), "Input:");

        /// <summary>
        /// Gets or sets the InputLabel property. This StyledProperty 
        /// indicates ....
        /// </summary>
        public string InputLabel
        {
            get => this.GetValue(InputLabelProperty);
            set => SetValue(InputLabelProperty, value);
        }
    }
}
