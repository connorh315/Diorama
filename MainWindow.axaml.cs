using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace Diorama
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.AttachDevTools();
        }

        private void Window_DragDrop(object sender, DragEventArgs e)
        {
            var firstFile = e.Data.GetFiles()?.FirstOrDefault();
            if (firstFile == null)
                return;

            MainViewport.LoadScene(firstFile.Path.LocalPath);
        }
    }
}