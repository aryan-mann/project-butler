using System.Windows;

namespace Butler {

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        
        public App() {
            //Shutdown when the main window or the last window closes
            ShutdownMode = ShutdownMode.OnMainWindowClose;
        }

    }
}
