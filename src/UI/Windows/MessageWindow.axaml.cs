using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Diorama.UI.Controls;
using Diorama.UI.Windows;

namespace Diorama;

public partial class MessageWindow : ModalWindow
{
    public MessageWindow(string title) : base(title)
    {
        InitializeComponent();
    }

    public MessageWindow(string title, IEnumerable<string> messages) : this(title)
    {
        Title = title;
        MessageText.Text = string.Join(Environment.NewLine, messages);
    }
}