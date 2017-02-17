using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
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
                pointPerModule = (90/count);
                Dispatcher.Invoke(() => {
                    Status.Text = "Loading modules";
                });
            };

            ModuleLoader.ModuleLoaded += module => {
                Dispatcher.Invoke(() => {
                    Status.Text = module.Name + " loaded";
                    Progress.Value += pointPerModule;
                });
            };

            ModuleLoader.LoadingEnded += () => {
                Dispatcher.Invoke(() => {
                    Progress.Value = 100;
                    Status.Text = "Loaded all modules";
                    new MainWindow().Show();
                    Close();
                });
            };

            Task.Factory.StartNew(ModuleLoader.LoadAll);
        }
    }
}
