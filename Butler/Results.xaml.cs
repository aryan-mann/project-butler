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

namespace Butler {

    /// <summary>
    /// Interaction logic for Results.xaml
    /// </summary>
    public partial class Results : Window {

        public Results() {
            InitializeComponent();

            Deactivated += Shutdown;
            this.KeyDown += Results_KeyDown;
        }

        private void Results_KeyDown(object sender, KeyEventArgs e) {
            if(e.Key == Key.Escape || e.Key == Key.Enter) {
                Close();
            }
        }

        private void Shutdown(object sender, EventArgs e) {
            if(this.ShowActivated)
            Close();
        }

        public void SetupData<T>(List<T> data) {
            DataList.ItemsSource = data;
        }
    }
}
