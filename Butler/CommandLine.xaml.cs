using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Butler {

    /// <summary>
    /// Interaction logic for CommandLine.xaml
    /// </summary>

    public partial class CommandLine : Window {

        static CommandLine CmdInstance = null; 

        private CommandLine() {
            InitializeComponent();

            this.SourceInitialized += CommandLine_SourceInitialized;
            this.Activated += CommandLine_Activated;
            this.GotFocus += CommandLine_Activated;

            //Hide when first shown
            this.Loaded += (sender, e) => {
                Hide();
            };

            this.PreviewKeyDown += CommandLine_PreviewKeyDown;
            this.Loaded += (sender, e) => {
                Hide();
            };

            this.Deactivated += (sender, e) => {
                Hide();
            };
        }

        private void CommandLine_PreviewKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Escape) {
                Visibility = Visibility.Collapsed;
                e.Handled = true;
            } else if (e.Key == Key.Enter && Input.IsKeyboardFocused) {
                InitiateCommand();
                e.Handled = true;
            }
        }

        private void InitiateCommand() {
            string _query = Input.Text;
            if (string.IsNullOrWhiteSpace(_query)) {
                CurrentStatus.Content = "Empty Command!";
                return;
            }

            UserModule selectedModule = null;
            string selectedRegexKey = "";
            bool matchFound = false;

            foreach(UserModule mod in ModuleLoader.ModuleLoadOrder.Values) {
                mod.RegisteredCommands.ToList().ForEach(kvp => {

                    if(kvp.Value.Match(_query).Success) {
                        selectedModule = mod;
                        selectedRegexKey = kvp.Key;
                        matchFound = true;
                        return;
                    }
                });
                if(matchFound) { break; }
            }

            if(!matchFound || selectedModule == null || string.IsNullOrWhiteSpace(selectedRegexKey)) {
                CurrentStatus.Content = "Couldn't find that command";
                return;
            }

            CmdInstance.Hide();
                        
            selectedModule.GiveRegexCommand(selectedRegexKey, _query);
        }

        private void CommandLine_Activated(object sender, EventArgs e) {
            this.Activate();
            Input.Focus();
            Keyboard.ClearFocus();
            Keyboard.Focus(Input);
            Input.SelectAll();
        }

        private void CommandLine_SourceInitialized(object sender, EventArgs e) {
            Rect workArea = SystemParameters.WorkArea;
            this.Left = (workArea.Width - this.Width) / 2 + workArea.Left;
            this.Top = (workArea.Height - this.Height) / 2 + workArea.Top;
        }

        public static CommandLine GetInstance() {
            if (CmdInstance == null) {
                CmdInstance = new CommandLine();
            }
            return CmdInstance;
        }
        
    }
}
