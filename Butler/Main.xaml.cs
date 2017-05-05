using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ModuleAPI;
using Application = System.Windows.Application;

namespace Butler {

    public partial class MainWindow: Window {

        //Singular Instance of the User Input prompt
        private CommandLine Cmd { get; set; } = CommandLine.GetInstance();
        public bool Ready = false;

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

            Closing += (o, args) => {
                Application.Current.Shutdown();
            };

            //Setup the command line interface
            Cmd = Cmd ?? CommandLine.GetInstance();

            //Taskbar notification icon
            TaskbarIconManager.AddItem("Show", () => {
                ShowInTaskbar = true;
                Visibility = Visibility.Visible;
                Activate();
                WindowState = WindowState.Maximized;
            });
            TaskbarIconManager.AddItem("Exit", () => {
                Application.Current.Shutdown(0);
            });
            TaskbarIconManager.CommitItems();
            TaskbarIconManager.SetVisible(true);

            LoadedModules.SelectionChanged += LoadedModulesOnSelectionChanged;

            // Invokes commands recieved from TCP connections
            RemoteControlManager.CommandRecieved += (command, tcpClient) => {

                Task.Factory.StartNew(() => {
                    UserModule mod = null;
                    Command cm = null;

                    if(UserModule.FindResponsibleUserModule(command, out mod, out cm, tcpClient)) {
                        Dispatcher.Invoke(() => {
                            StatusMessage.Content = $"{tcpClient.Client.RemoteEndPoint} > [{mod.Name}] {cm.LocalCommand}";
                            mod.GiveRegexCommand(cm);
                        });
                    }
                });

            };

            RemoteControlManager.ClientConnected += client => {
                Dispatcher.Invoke(() => {
                    StatusMessage.Content = $"({client.Client.RemoteEndPoint}) has connected!";
                });
            };
            RemoteControlManager.ClientDisconnected += client => {
                Dispatcher.Invoke(() => {
                    StatusMessage.Content = $"({client.Client.RemoteEndPoint}) has now disconnected.";
                });
            };
        }

        // New item from Loaded Modules selected
        private void LoadedModulesOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs) {
            ListBoxItem item = (ListBoxItem) LoadedModules.SelectedItem;
            if(item == null) { return; }

            KeyValuePair<int, UserModule> val = (KeyValuePair<int, UserModule>) item.DataContext;
            if(val.Value == null) { return; }

            DisplayInformationFor(val.Value);
        }

        // Changes UI based on which Module is selected
        private void DisplayInformationFor(UserModule um) {
            UmName.Content = um.Name;
            UmCommands.Items.Clear();

            foreach(KeyValuePair<string, Regex> pair in um.RegisteredCommands) {
                UmCommands.Items.Add(new ListBoxItem() {
                    Content = pair.Value,
                    DataContext = pair
                });
            }

            UmVersion.Content = um.SemVer;
            UmAuthor.Content = um.Author;
            UmWebsite.Content = um.Website;
            UmDirectory.Content = um.BaseDirectory;
            UmCommandCount.Content = um.RegisteredCommands.Count.ToString();
            UmTrigger.Content = um.Prefix;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e) {
            //Load all modules on application start
            //ModuleLoader.LoadAll();
            SyncModules();


        }

        // Syncs ModuleLoader.ModuleLoadOrder and LoadedModules.Items
        private void SyncModules() {
            LoadedModules.Items.Clear();
            foreach(KeyValuePair<int, UserModule> pair in ModuleLoader.ModuleLoadOrder) {
                LoadedModules.Items.Add(new ListBoxItem() {
                    Content = pair.Value.Name,
                    DataContext = pair
                });
            }
        }

        /* Right Menu Button Events (START) */
        private void AboutButton_Click(object sender, RoutedEventArgs e) {
            MessageBox.Show("Developed By Aryan Mann", "Hey!", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void RemoteButton_Click(object sender, RoutedEventArgs e) {
            if(!RemoteControlManager.ServerRunning) {
                RemoteButton.Background = (Brush) Application.Current.Resources["On"];
                RemoteControlManager.StartServer();
            } else {
                RemoteButton.Background = (Brush) Application.Current.Resources["Off"];
                RemoteControlManager.StopServer();
            }

            StatusMessage.Content = $"The server is now {(RemoteControlManager.ServerRunning ? "" : "not")} running.";
        }
        /* Right Menu Button Events (END) */

        // Current Hotkey = WinKey + Escape
        private void HotkeyWasPressed() {

            //Toggle command line visibilty on global hotkey press
            switch(Cmd.Visibility) {
                case Visibility.Visible:
                    Cmd.Hide();
                    break;
                case Visibility.Hidden:
                case Visibility.Collapsed:
                    Cmd.Show();
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
        }


        // Hide/Show in taskbar depending on window visibility
        private void MainWindow_StateChanged(object sender, EventArgs e) {
            switch(WindowState) {
                case WindowState.Minimized:
                    ShowInTaskbar = false;
                    Visibility = Visibility.Hidden;
                    break;
                case WindowState.Maximized:
                case WindowState.Normal:
                    ShowInTaskbar = true;
                    Visibility = Visibility.Visible;
                    break;
            }
        }


    }
}
