using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Browser {

    /// <summary>
    /// Interaction logic for WebBrowser.xaml
    /// </summary>
    public partial class WebBrowser : Window {

        public delegate void OnNavigated(string url);
        public event OnNavigated Navigated;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, long dwNewLong);

        public WebBrowser() {
            InitializeComponent();

            Height = SystemParameters.PrimaryScreenHeight/1.4;
            Width = SystemParameters.PrimaryScreenWidth/1.4;

            WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        
            KeyDown += (sender, e) => {
                if(e.Key == Key.Escape) {
                    UWeb.Dispose();
                    Close();
                }
            };
            
            UWeb.Navigated += (sender, e) => {
                //Hack so that the 'load javascript?' prompt does not show up
                dynamic activeX = UWeb.GetType().InvokeMember("ActiveXInstance",
                    BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                    null, UWeb, new object[] { });
                activeX.Silent = true;                

                Navigated?.Invoke(e.Uri.ToString());
                NavUrl.Text = e.Uri.ToString();
            };

            NavUrl.KeyDown += (sender, e) => {
                if(e.Key == Key.Enter) {
                    e.Handled = true;
                    try {
                        ShowPage(NavUrl.Text);
                    } catch(Exception ex) { Console.WriteLine(ex.Message); }
                }
            };

            Loaded += (sender, e) => {
                IntPtr hWnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
                //If you allow transparency on the window, the web browser does not render
                //By using WinAPI, we can bypass that bug by setting the window properties manually
                SetWindowLong(hWnd, -16, 0x80000000);
            };
        }

        public void ShowPage(string url) {
            Visibility = Visibility.Visible;
            Match m = Regex.Match(url, @"(https?|ftp):\/\/[^\s/$.?#].[^\s]*");
            if(m.Success) {
                UWeb.Navigate(m.ToString());
            } else {
                UWeb.Navigate(@"https://www.duckduckgo.com/?q=" + url);
            }
        }

    }

}
