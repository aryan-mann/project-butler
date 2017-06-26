using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using Butler.Annotations;
using static System.Diagnostics.Process;
using static System.IO.Path;
using static System.Reflection.Assembly;

namespace Butler
{

    /// <summary>
    /// Interaction logic for Loader.xaml
    /// </summary>
    public partial class Loader : Window, INotifyPropertyChanged
    {
        public Loader() {
            InitializeComponent();
            Loaded += Loader_LoadedAsync;
        }

        private string _message = "Checking if another instance is running";
        public string Message {
            get { return _message + ".."; }
            private set { _message = value; OnPropertyChanged(); }
        }

        private async void Loader_LoadedAsync(object sender, RoutedEventArgs e) {
            Application.Current.ShutdownMode = ShutdownMode.OnLastWindowClose;

            if(GetProcessesByName(GetFileNameWithoutExtension(GetEntryAssembly().Location)).Length > 1) {
                MessageBox.Show("An instance of this application is already running.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }

            Progress.Value = 10;

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
                    Progress.Value += pointPerModule;
                });
            };

            ModuleLoader.LoadingEnded += () => {
                Dispatcher.Invoke(() => {
                    Progress.Value = 100;
                    Message = "Loaded all modules";
                    new MainWindow().Show();
                    Close();
                });
            };

            await ModuleLoader.LoadAllAsync();
        }


        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
