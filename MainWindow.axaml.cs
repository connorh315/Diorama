using Avalonia;
using Avalonia.Controls;

namespace Diorama
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.AttachDevTools();
        }
    }
}