using System;
using System.Windows;
using System.Windows.Input;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace Butler {

    /// <summary>
    /// Interaction logic for CommandLine.xaml
    /// </summary>
    public partial class CommandLine: Window {

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

            //Hide when focus is lost
            Deactivated += (sender, e) => {
                Hide();
            };
        }

        // Control window visibility using keyboard
        private void CommandLine_PreviewKeyDown(object sender, KeyEventArgs e) {
            if(e.Key == Key.Escape) {
                Visibility = Visibility.Collapsed;
                e.Handled = true;
            } else if(e.Key == Key.Enter && Input.IsKeyboardFocused) {
                InitiateCommand();
                e.Handled = true;
            }
        }


        /* Searches through all modules to see if any one of their registered regex's 
        matches the user input, if it does, we invoke the OnCommandReceived function in the
        modules hook class */
        private void InitiateCommand() {
            string query = Input.Text;
            if (UserModule.FindAndGiveRegexCommand(query)) {
                _cmdInstance.Hide();
            }
        }
        
        // When window is shown, put focus on the command text
        private void CommandLine_Activated(object sender, EventArgs e) {
            Activate();
            Input.Focus();
            Keyboard.ClearFocus();
            Keyboard.Focus(Input);
            Input.SelectAll();
        }

        // Place textbox on the bottom of the screen
        private void CommandLine_SourceInitialized(object sender, EventArgs e) {
            Width = SystemParameters.PrimaryScreenWidth;
            Left = 0;
            Top = SystemParameters.PrimaryScreenHeight - Height;
        }


    }
}
