using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using MusicPlayer;

namespace MusicBrowser {
    /// <summary>
    /// Interaction logic for SearchResult.xaml
    /// </summary>
    public partial class SearchResult : Window {

        public enum Method {
            ListAll
        }
        public Method SearchMethod { get; set; } = Method.ListAll;
        
        public SearchResult() {
            InitializeComponent();
            Loaded += SearchResult_Loaded;
            SearchData.MouseDoubleClick += SearchData_MouseDoubleClick;
        }

        private void SearchData_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            var val = SearchData.SelectedValue;
            if(val is Song) {
                Process proc = Process.Start(((Song)val).Filepath);
                Close();
            }
        }

        private void SearchResult_Loaded(object sender, RoutedEventArgs e) {
            if(SearchMethod == Method.ListAll) {
                SearchData.ItemsSource = Song.SongList;
            }
        }
    }
}
