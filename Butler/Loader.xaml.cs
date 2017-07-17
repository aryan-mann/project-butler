using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Butler.Annotations;
using Butler.Properties;
using Newtonsoft.Json.Linq;
using static System.Diagnostics.Process;
using static System.IO.Path;
using static System.Reflection.Assembly;
using Settings = ModuleAPI.Settings;

namespace Butler {

    /// <summary>
    /// Interaction logic for Loader.xaml
    /// </summary>
    public partial class Loader: Window, INotifyPropertyChanged {
        public Loader() {
            InitializeComponent();
            Loaded += Loader_LoadedAsync;
        }

        private string _message = "Checking if another instance is running";
        public string Message {
            get { return _message + ".."; }
            private set { _message = value; OnPropertyChanged(); }
        }

        private double _progress;
        public double Progress {
            get { return _progress; }
            set { _progress = value; OnPropertyChanged(); }
        }

        private void Loader_LoadedAsync(object sender, RoutedEventArgs e) {
            Application.Current.ShutdownMode = ShutdownMode.OnLastWindowClose;

            if(GetProcessesByName(GetFileNameWithoutExtension(GetEntryAssembly().Location)).Length > 1) {
                MessageBox.Show("An instance of this application is already running.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }

            Progress = 10;

            double pointPerModule = 0;
            ModuleLoader.LoadingStarted += count => {
                pointPerModule = 90f / count;
                Dispatcher.Invoke(() => {
                    Message = "Loading modules";
                });
            };

            ModuleLoader.ModuleLoaded += module => {
                Dispatcher.Invoke(() => {
                    Message = module.Name + " loaded";
                    Progress += pointPerModule;
                    Logger.Log($"Loaded {module.Name}");
                });
            };

            ModuleLoader.LoadingEnded += () => {
                Dispatcher.Invoke(async () => {
                    Progress = 100;
                    Message = "Loading preferences";
                    await CheckPreferences();
                });
            };

            Task.Run(async () => await ModuleLoader.LoadAllAsync());
        }

        private async Task CheckPreferences() {
            var loadSettings = await Task.Run(() => Settings.CreateOrLoadSettings("Preferences", AppDomain.CurrentDomain.BaseDirectory));
            
            if(loadSettings != null) {

                while (true) {
                    try {
                        await Task.Run(() => loadSettings.Load());
                        goto outloop;
                    } catch { Task.Delay(250).Wait(); }
                }

                outloop:
                var activeStatus = loadSettings.Get<JObject>("ActiveStatus");
                if(activeStatus != null) {
                    foreach (var keyValuePair in activeStatus) {
                        var module = ModuleLoader.ModuleLoadOrder.FirstOrDefault(v => v.Value.Name == keyValuePair.Key);
                        if(module.Value != null) { module.Value.Enabled = keyValuePair.Value.Value<bool>(); }
                    }
                }
            }
            
            new MainWindow().Show();
            Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
