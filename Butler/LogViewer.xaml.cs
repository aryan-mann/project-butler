using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Butler
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
