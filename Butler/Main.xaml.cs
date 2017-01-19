using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace Butler {

    public partial class MainWindow : Window {

        //Singular Instance of the User Input prompt
        CommandLine CMD;
        public bool Ready = false;
        List<int> SelectedLoadOrders = new List<int>();

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

        private void SetupComponents(object sender, RoutedEventArgs e) {
            //Setup the command line interface
            CMD = CommandLine.GetInstance();

            //Taskbar notification icon
            TaskbarIconManager.AddItem("Show", () => {
                WindowState = WindowState.Normal;
                ShowInTaskbar = true;
                Visibility = Visibility.Visible;
            });
            TaskbarIconManager.AddItem("Exit", () => {
                System.Windows.Application.Current.Shutdown(0);
            });

            TaskbarIconManager.CommitItems();
            TaskbarIconManager.SetVisible(true);
        }
                
        private void MainWindow_Loaded(object sender, RoutedEventArgs e) {
            //Load all modules on application start
            ModuleLoader.LoadAll();
            ModuleDataGrid.ItemsSource = ModuleLoader.ModuleLoadOrder;
        }
        
        //Current Hotkey = WinKey + Escape
        private void HotkeyWasPressed() {
            
            //Toggle command line visibilty on global hotkey press
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

            //When the application is closing, remove the taskbar icon
            this.Closed += (sender, e) => {
                TaskbarIconManager.Dispose();
            };

            this.Loaded += SetupComponents;
            this.StateChanged += MainWindow_StateChanged;

            this.ModuleDataGrid.SelectionChanged += ModuleDataGrid_SelectionChanged;

            // Enables/Disables the selected modules
            this.Bu_ToggleActive.Click += (sender, e) => {
                SelectedLoadOrders.ForEach(ld => {
                    ModuleLoader.ModuleLoadOrder[ld].Enabled = !ModuleLoader.ModuleLoadOrder[ld].Enabled;
                });

                ModuleDataGrid.ItemsSource = null;
                ModuleDataGrid.ItemsSource = ModuleLoader.ModuleLoadOrder;
            };
        }

        //Menu bar button events
        private void MenuAbout_Clicked(object sender, RoutedEventArgs e) {
            System.Windows.MessageBox.Show("Project Butler was developed by Aryan Mann", "Hey!", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void MenuExit_Clicked(object sender, RoutedEventArgs e) {
            System.Windows.Application.Current.Shutdown(0);
        }

        //Manage selected modules when selection is changed
        private void ModuleDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            
            //Disable Toggle Active button if there are no items selected
            Bu_ToggleActive.IsEnabled = !(ModuleDataGrid.SelectedItems.Count == 0);
            if(ModuleDataGrid.SelectedItems.Count == 0) { return; }

            SelectedLoadOrders = new List<int>();
            foreach(var kvp in ModuleDataGrid.SelectedItems) {
                SelectedLoadOrders.Add(((KeyValuePair<int, UserModule>)kvp).Key);
            }
        }

        //Hide/Show in taskbar depending on window visibility
        private void MainWindow_StateChanged(object sender, EventArgs e) {
            switch(this.WindowState) {
                case WindowState.Minimized: ShowInTaskbar = false;
                    Visibility = Visibility.Hidden;
                    break;
                case WindowState.Maximized:
                case WindowState.Normal: ShowInTaskbar = true;
                    Visibility = Visibility.Visible;
                    break;
            }
        }

        
    }
}
