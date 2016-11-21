using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace Youtube {

    /// <summary>
    /// Interaction logic for YoutubeVideo.xaml
    /// </summary>
    public partial class YoutubeVideo : Window {

        const string API_KEY = "AIzaSyDmZ5rGzV38mrGfcSMPegvx8xxndSHmnT4";
        
        string videoID = "";
        string videoUrl {
            get {
                return $@"https://www.youtube.com/embed/{videoID}?autoplay=1&fs=0&modestbranding=1&showinfo=0";
            }
        }
        string queryUrl {
            get {
                return $@"https://www.googleapis.com/youtube/v3/videos?id={videoID}&key={API_KEY}&part=status&fields=items(id,status(uploadStatus,privacyStatus))";
            }
        }
        
        public YoutubeVideo(string _videoID) {
            InitializeComponent();
            
            videoID = _videoID;
            Loaded += YoutubeVideo_Loaded;

            Browser.Navigated += Browser_Navigated;

            Closing += YoutubeVideo_Closing;
        }

        private void Browser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e) {
            Browser.Navigated += (s, e2) => {
                dynamic activeX = Browser.GetType().InvokeMember("ActiveXInstance",
                    BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                    null, Browser, new object[] { });
                activeX.Silent = true;
            };
        }

        private void YoutubeVideo_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            Browser.Navigate("http://www.google.com/");
        }

        private void YoutubeVideo_Loaded(object sender, RoutedEventArgs e) {
            Browser.Navigate(videoUrl);
        }
    }
}
