using System.Threading.Tasks;
using System.Windows;
using static System.Diagnostics.Process;
using static System.IO.Path;
using static System.Reflection.Assembly;

namespace Butler
{

    /// <summary>
    /// Interaction logic for Loader.xaml
    /// </summary>
    public partial class Loader : Window
    {
        public Loader() {
            InitializeComponent();
            Loaded += Loader_Loaded;
        }

        private void Loader_Loaded(object sender, RoutedEventArgs e) {

            Application.Current.ShutdownMode = ShutdownMode.OnLastWindowClose;

            if (GetProcessesByName(GetFileNameWithoutExtension(GetEntryAssembly().Location)).Length > 1) {
                MessageBox.Show("An instance of this application is already running.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }

            Progress.Value = 10;

            double pointPerModule = 0;
            ModuleLoader.LoadingStarted += count => {
                pointPerModule = 90f/count;
                Dispatcher.Invoke(() => {
                    Status.Content = "Loading modules";
                });
            };

            ModuleLoader.ModuleLoaded += module => {
                Dispatcher.Invoke(() => {
                    Status.Content = module.Name + " loaded";
                    Progress.Value += pointPerModule;
                });
            };

            ModuleLoader.LoadingEnded += () => {
                Dispatcher.Invoke(() => {
                    Progress.Value = 100;
                    Status.Content = "Loaded all modules";
                    new MainWindow().Show();
                    Close();
                });
            };

            Task.Factory.StartNew(ModuleLoader.LoadAll);
        }
    }
}
