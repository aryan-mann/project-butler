using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ModuleAPI;

namespace Browser {

    [ApplicationHook]
    public class Hook : Module {

        public override string Author {
            get { return "Aryan Mann"; }
        }
        public override string Name {
            get { return "Simple Browser"; }
        }
        public override Dictionary<string, Regex> RegisteredCommands {
            get {
                return new Dictionary<string, Regex>() {
                    ["url"] = new Regex(@"url (?<url>.+)")
                };
            }
        }
        public override string SemVer {
            get { return "0.1.0"; }
        }
        public override Uri Website {
            get { return new Uri("http://www.aryanmann.com/"); }
        }

        public override void ConfigureSettings() {
            return;
        }

        public override void OnCommandRecieved(string CommandName, string UserInput) {
            if(CommandName == "url") {
                Match m = RegisteredCommands["url"].Match(UserInput);
                System.Windows.MessageBox.Show(m.Groups["url"].Value, "I got what you said!");
            }
        }

        public override void OnInitialized() {
            return;
        }

        public override void OnShutdown() {
            return;
        }
    }
}
