using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Diorama
{
    public class LabelledInput : ContentControl, INotifyPropertyChanged
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

        //public static readonly StyledProperty<string> WarningProperty =
        //    AvaloniaProperty.Register<LabelledInput, string>(nameof(InputLabel));

        //public string Warning
        //{
        //    get => this.GetValue(WarningProperty);
        //    set => SetValue(WarningProperty, value);
        //}

        public string Warning { get; set; }
        public bool ShowWarning { get; set; } = false;

        public void SetWarning(string warning)
        {
            Warning = warning;
            ShowWarning = true;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
