using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;

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

#if DEBUG // When triggering a breakpoint, explorer sort of just freezes until the code continues which is insufferable
            Dispatcher.UIThread.Invoke(new Action(() =>
            {
#endif
                MainViewport.LoadScene(firstFile.Path.LocalPath);
#if DEBUG
            }), DispatcherPriority.Background);
#endif
        }
    }
}