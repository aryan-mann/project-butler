using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Butler {

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        
        public App() {
            //Shutdown when the main window or the last window closes
            ShutdownMode = ShutdownMode.OnLastWindowClose | ShutdownMode.OnMainWindowClose;
        }

    }
}
