using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace Butler {

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        CommandLine CMD;
        public bool Ready = false;

        #region Hotkey Registration
        HotkeyHandler kHandler;
        private void Main_HotkeyRegister(object sender, EventArgs e) {
            kHandler = new HotkeyHandler(this);
            kHandler.HotkeyPressed += HotkeyWasPressed;
        }
        private void Main_HotkeyDeregister(object sender, System.ComponentModel.CancelEventArgs e) {
            kHandler.Dispose();
        }
        #endregion
        //Current Hotkey = WinKey + Escape
        private void HotkeyWasPressed() {
            switch(CMD.Visibility) {
                case Visibility.Visible:
                CMD.Hide();
                break;
                case Visibility.Hidden:
                case Visibility.Collapsed:
                CMD.Show();
                break;
            }

        }


        public MainWindow() {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;

            this.SourceInitialized += Main_HotkeyRegister;  //Inside Hotkey Registration Region
            this.Closing += Main_HotkeyDeregister;          //Inside Hotkey Registration Region

            this.Closing += (sender, e) => {
                Application.Current.Shutdown(0);
            };

            this.Loaded += SetupComponents;
        }

        private void SetupComponents(object sender, RoutedEventArgs e) {
            CMD = CommandLine.GetInstance();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e) {
            ModuleLoader.LoadAll();
            ModuleDataGrid.ItemsSource = ModuleLoader.ModuleLoadOrder;
        }

        private void ReloadDataGridUI(bool UIThread = false) {
            
        }

    }
}
