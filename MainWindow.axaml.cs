using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Diorama.Rendering;
using Diorama.UI.Controls;

namespace Diorama
{
    public partial class MainWindow : Window
    {
        private SceneController sceneController;
        
        private ViewportNewControl MainViewport;
        private SceneHierarchy Hierarchy;
        private InspectorPanel Inspector;
        private GeometryObjectPanel Geometry;

        public MainWindow()
        {
            InitializeComponent();

            sceneController = new SceneController(new ViewportRenderer());

            MainViewport = new ViewportNewControl(sceneController);
            ViewportHost.Content = MainViewport;

            Hierarchy = new SceneHierarchy(sceneController);
            HierarchyHost.Content = Hierarchy;

            Inspector = new InspectorPanel(sceneController);
            InspectorHost.Content = Inspector;

            Geometry = new GeometryObjectPanel(sceneController);
            GeometryHost.Content = Geometry;

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

        private void MenuItem_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ViewportNewControl.ShowLightmaps = !ViewportNewControl.ShowLightmaps;
        }
    }
}