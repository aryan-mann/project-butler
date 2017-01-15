using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Butler {

    /// <summary>
    /// Interaction logic for CommandLine.xaml
    /// </summary>
    public partial class CommandLine : Window {

        //Singleton-esque method of getting the command line 
        public static CommandLine GetInstance() {
            if (CmdInstance == null) {
                CmdInstance = new CommandLine();
            }
            return CmdInstance;
        }
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

            //Hide when focus is lost
            this.Deactivated += (sender, e) => {
                Hide();
            };
        }

        // Control window visibility using keyboard
        private void CommandLine_PreviewKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Escape) {
                Visibility = Visibility.Collapsed;
                e.Handled = true;
            } else if (e.Key == Key.Enter && Input.IsKeyboardFocused) {
                InitiateCommand();
                e.Handled = true;
            }
        }


        /* Searches through all modules to see if any one of their registered regex's 
        matches the user input, if it does, we invoke the OnCommandReceived function in the
        modules hook class */
        private void InitiateCommand() {
            string _query = Input.Text;
            if (string.IsNullOrWhiteSpace(_query)) {
                //CurrentStatus.Content = "Empty Command!";
                return;
            }

            UserModule selectedModule = null;
            string selectedRegexKey = "";
            bool matchFound = false;

            //Check if user input matches Regexes' of enabled Modules
            foreach(UserModule mod in ModuleLoader.ModuleLoadOrder.Values) {
                if(!mod.Enabled) { continue; }
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

            //If a match is not found or the user input is invalid, select all user input text
            if(!matchFound || selectedModule == null || string.IsNullOrWhiteSpace(selectedRegexKey)) {
                Input.SelectAll();
                return;
            }

            CmdInstance.Hide();
                        
            selectedModule.GiveRegexCommand(selectedRegexKey, _query);
        }

        // When window is shown, put focus on the command text
        private void CommandLine_Activated(object sender, EventArgs e) {
            this.Activate();
            Input.Focus();
            Keyboard.ClearFocus();
            Keyboard.Focus(Input);
            Input.SelectAll();
        }

        // Place textbox on the bottom of the screen
        private void CommandLine_SourceInitialized(object sender, EventArgs e) {
            this.Width = SystemParameters.PrimaryScreenWidth;
            this.Left = 0;
            this.Top = SystemParameters.PrimaryScreenHeight - this.Height;
        }

        
    }
}
