using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Butler.Core;
using Butler.DataStuctures;
using Butler.Properties;
using ModuleAPI;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace Butler.Views {

    /// <summary>
    /// Interaction logic for CommandLine.xaml
    /// </summary>
    public partial class CommandLine: Window, INotifyPropertyChanged {

        //Singleton-esque method of getting the command line 
        private static CommandLine _cmdInstance = new CommandLine();
        public static CommandLine GetInstance() {
            _cmdInstance = _cmdInstance ?? new CommandLine();
            return _cmdInstance;
        }
        private CommandLine() {
            InitializeComponent();
            
            SourceInitialized += CommandLine_SourceInitialized;
            Activated += CommandLine_Activated;
            GotFocus += CommandLine_Activated;

            //Hide when first shown
            Loaded += (sender, e) => {
                Hide();
            };
            
            PreviewKeyDown += CommandLine_PreviewKeyDown;

            Command.Responded += ResponseReceieved;

            //Hide when focus is lost
            Deactivated += (sender, e) => {
                Hide();
            };
        }

        private string _response;
        public string Response {
            get { return _response; }
            set { _response = value; OnPropertyChanged(); }
        }


        private void ResponseReceieved(string response, Command com, TcpClient client) {
            Response = $"{response}";
        }

        private readonly QueueList<string> _commandHistory = new QueueList<string>(10);

        // Control window visibility using keyboard
        private void CommandLine_PreviewKeyDown(object sender, KeyEventArgs e) {
            if(e.Key == Key.Escape) {
                Visibility = Visibility.Collapsed;
                e.Handled = true;
            } else if(e.Key == Key.Enter && Input.IsKeyboardFocused) {
                InitiateCommand();
                e.Handled = true;
            } else if (e.Key == Key.Up) {
                _commandHistory.SetToNextNode();
                Input.Text = _commandHistory.CurrentNode;
                e.Handled = true;
            } else if (e.Key == Key.Down) {
                _commandHistory.SetToPreviousNode();
                Input.Text = _commandHistory.CurrentNode;
            }
        }
        
        private void InitiateCommand() {
            var query = Input.Text;
            //_cmdInstance.Hide();

            Task.Run(() => {
                UserModule um = null;
                Command cm = null;

                if(UserModule.FindResponsibleUserModule(query, out um, out cm)) {
                    Dispatcher.Invoke(() => {
                        um.GiveRegexCommand(cm);
                        _commandHistory.Add(cm.ToString());
                    });
                }
            });

        }
        
        // When window is shown, put focus on the command text
        private void CommandLine_Activated(object sender, EventArgs e) {
            Activate();
            Focus();
            BringIntoView();    

            Keyboard.PrimaryDevice.Focus(Input);
            if (Input.Text == "Enter Command Here") {
                Input.SelectAll();
            } else {
                Input.SelectionStart = Input.Text.Length;
            }
        }

        // Place textbox on the bottom of the screen
        private void CommandLine_SourceInitialized(object sender, EventArgs e) {
            Width = SystemParameters.PrimaryScreenWidth;
            Left = 0;
            Top = SystemParameters.PrimaryScreenHeight - Height;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
