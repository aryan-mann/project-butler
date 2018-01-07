using System.Windows;

namespace Butler.Views
{
    /// <summary>
    /// Interaction logic for LogViewer.xaml
    /// </summary>
    public partial class LogViewer : Window {

        private static LogViewer _instance;
        public static LogViewer Instance => _instance = (_instance ?? new LogViewer());
        private LogViewer() {
            InitializeComponent();
            Closing += (sender, args) => { args.Cancel = true; Hide(); };
        }

        public Logger Logger => Logger.Instance;

        public static void Display() {
            Instance.Show();
        }

    }
}
