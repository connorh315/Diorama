using Avalonia.Controls;
using Avalonia.Input;
using Diorama.UI.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.UI.Windows
{
    public class ModalWindow : Window
    {
        public ModalWindow(string title)
        {
            SizeToContent = SizeToContent.WidthAndHeight;
            CanResize = false;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            Title = title;
        }

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);

            var hwnd = this.TryGetPlatformHandle();

            if (hwnd != null)
            {
                Win32.RemoveMinMaxButtons(hwnd.Handle);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.Escape)
                Close();
        }
    }
}
