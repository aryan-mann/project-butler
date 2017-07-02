using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Butler.Annotations;
using ModuleAPI;
using Application = System.Windows.Application;

namespace Butler {

    public partial class MainWindow: Window, INotifyPropertyChanged {

        //Singular Instance of the User Input prompt
        private CommandLine Cmd { get; set; } = CommandLine.GetInstance();
        public bool Ready = false;

        private string _status = "";
        public string Status {
            get { return _status; }
            private set { _status = value; OnPropertyChanged(); }
        }

        private UserModule _selectedUserModule = null;
        public UserModule SelectedUserModule {
            get { return _selectedUserModule; }
            private set {
                _selectedUserModule = value; 
                OnPropertyChanged();
            }
        }

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

        public RemoteControlManager RemoteManager { get; } = RemoteControlManager.Instance;

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
            RemoteManager.CommandRecieved += (command, tcpClient) => {

                Task.Factory.StartNew(() => {
                    UserModule mod = null;
                    Command cm = null;

                    if(UserModule.FindResponsibleUserModule(command, out mod, out cm, tcpClient)) {
                        Dispatcher.Invoke(() => {
                            Status = $"{(tcpClient != null ? tcpClient.Client.RemoteEndPoint + " > " : "")} [{mod.Name}:{mod.Prefix}] > {cm.LocalCommand}";
                            mod.GiveRegexCommand(cm);
                        });
                    }
                });

            };

            RemoteManager.ClientConnected += client => {
                Dispatcher.Invoke(() => {
                    Status = $"({client.Client.RemoteEndPoint}) has connected!";
                });
            };
            RemoteManager.ClientDisconnected += client => {
                Dispatcher.Invoke(() => {
                    Status = $"({client.Client.RemoteEndPoint}) has disconnected.";
                });
            };

            Command.Responded += (response, com, client) => {
                Dispatcher.Invoke(() => {
                    Status = $"[{com.UserModuleName}] > {response}";
                });
            };
        }
        
        // New item from Loaded Modules selected
        private void LoadedModulesOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs) {
            SelectedUserModule = LoadedModules.SelectedItem as UserModule;
        }
        
        private void MainWindow_Loaded(object sender, RoutedEventArgs e) {
            //Load all modules on application start
            //ModuleLoader.LoadAll();
            //SyncModules();
        }
        
        /* Right Menu Button Events (START) */
        private void AboutButton_Click(object sender, RoutedEventArgs e) {
            MessageBox.Show("Developed By Aryan Mann", "Hey!", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void RemoteButton_Click(object sender, RoutedEventArgs e) {
            if(!RemoteManager.ServerRunning) {
                RemoteManager.StartServer();
            } else {
                RemoteManager.StopServer();
            }

            Status = (RemoteManager.ServerRunning ? "Started" : "Stopped") + " the server.";
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

        private void ConfigureSelectedUserModule(object sender, RoutedEventArgs e) {
            SelectedUserModule?.GiveConfigureSettingsCommand(); }

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

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ShowLogs_OnClick(object sender, RoutedEventArgs e) {
            MessageBox.Show(Logger.LogSet.FirstOrDefault()?.Exception.Message);
        }
    }
}
