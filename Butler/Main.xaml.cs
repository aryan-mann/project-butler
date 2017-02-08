using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Butler {

    public partial class MainWindow : Window {

        //Singular Instance of the User Input prompt
        CommandLine _cmd;
        public bool Ready = false;
        List<int> _selectedLoadOrders = new List<int>();

        #region Hotkey Registration
        HotkeyHandler _kHandler;
        private void Main_HotkeyRegister(object sender, EventArgs e) {
            _kHandler = new HotkeyHandler(this);
            _kHandler.HotkeyPressed += HotkeyWasPressed;
        }
        private void Main_HotkeyDeregister(object sender, System.ComponentModel.CancelEventArgs e) {
            _kHandler.Dispose();
        }
        #endregion

        private void SetupComponents(object sender, RoutedEventArgs e) {
            //Setup the command line interface
            _cmd = CommandLine.GetInstance();

            //Taskbar notification icon
            TaskbarIconManager.AddItem("Show", () => {
                WindowState = WindowState.Normal;
                ShowInTaskbar = true;
                Visibility = Visibility.Visible;
            });
            TaskbarIconManager.AddItem("Exit", () => {
                Application.Current.Shutdown(0);
            });

            TaskbarIconManager.CommitItems();
            TaskbarIconManager.SetVisible(true);
        }
                
        private void MainWindow_Loaded(object sender, RoutedEventArgs e) {
            //Load all modules on application start
            try {
                ModuleLoader.LoadAll();
            } catch(System.Reflection.ReflectionTypeLoadException rl) {
                MessageBox.Show(rl.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            ModuleDataGrid.ItemsSource = ModuleLoader.ModuleLoadOrder;
        }
        
        //Current Hotkey = WinKey + Escape
        private void HotkeyWasPressed() {
            
            //Toggle command line visibilty on global hotkey press
            switch(_cmd.Visibility) {
                case Visibility.Visible:
                _cmd.Hide();
                break;
                case Visibility.Hidden:
                case Visibility.Collapsed:
                _cmd.Show();
                break;
            }
            
        }
        
        public MainWindow() {
            InitializeComponent();
            Loaded += MainWindow_Loaded;

            SourceInitialized += Main_HotkeyRegister;  //Inside Hotkey Registration Region
            Closing += Main_HotkeyDeregister;          //Inside Hotkey Registration Region

            //When the application is closing, remove the taskbar icon
            Closed += (sender, e) => {
                TaskbarIconManager.Dispose();
            };

            Loaded += SetupComponents;
            StateChanged += MainWindow_StateChanged;

            ModuleDataGrid.SelectionChanged += ModuleDataGrid_SelectionChanged;

            // Enables/Disables the selected modules
            BuToggleActive.Click += (sender, e) => {
                _selectedLoadOrders.ForEach(ld => {
                    ModuleLoader.ModuleLoadOrder[ld].Enabled = !ModuleLoader.ModuleLoadOrder[ld].Enabled;
                });

                ModuleDataGrid.ItemsSource = null;
                ModuleDataGrid.ItemsSource = ModuleLoader.ModuleLoadOrder;
            };
        }

        //Menu bar button events
        private void MenuAbout_Clicked(object sender, RoutedEventArgs e) {
            MessageBox.Show("Project Butler was developed by Aryan Mann", "Hey!", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void MenuExit_Clicked(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown(0);
        }

        //Manage selected modules when selection is changed
        private void ModuleDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            
            //Disable Toggle Active button if there are no items selected
            BuToggleActive.IsEnabled = (ModuleDataGrid.SelectedItems.Count != 0);
            if(ModuleDataGrid.SelectedItems.Count == 0) { return; }

            _selectedLoadOrders = new List<int>();
            foreach(var kvp in ModuleDataGrid.SelectedItems) {
                _selectedLoadOrders.Add(((KeyValuePair<int, UserModule>)kvp).Key);
            }
        }

        //Hide/Show in taskbar depending on window visibility
        private void MainWindow_StateChanged(object sender, EventArgs e) {
            switch(WindowState) {
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
