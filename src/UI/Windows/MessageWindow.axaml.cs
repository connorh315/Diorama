using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Diorama.UI.Controls;

namespace Diorama;

public partial class MessageWindow : Window
{
    public MessageWindow()
    {
        InitializeComponent();

        SizeToContent = SizeToContent.WidthAndHeight;
        CanResize = false;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
    }

    public MessageWindow(string title, IEnumerable<string> messages) : this()
    {
        Title = title;
        MessageText.Text = string.Join(Environment.NewLine, messages);
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
}