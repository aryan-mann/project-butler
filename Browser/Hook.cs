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

        #region Properties
        public override string Author {
            get { return "Aryan Mann"; }
        }
        public override string Name {
            get { return "Simple Browser"; }
        }
        public override Dictionary<string, Regex> RegisteredCommands {
            get {
                return new Dictionary<string, Regex>() {
                    ["search"] = new Regex(@"^search (?<url>.+)$"),
                    ["resume"] = new Regex(@"^sr$")
                };
            }
        }
        public override string SemVer {
            get { return "0.1.0"; }
        }
        public override Uri Website {
            get { return new Uri("http://www.aryanmann.com/"); }
        }
        #endregion

        //For resuming browsing.
        string LastUrl = "";

        WebBrowser _Instance = new WebBrowser();
        WebBrowser Instance {
            get {
                if(!_Instance.IsLoaded) {
                    _Instance = new WebBrowser();
                }
                _Instance.Navigated += (uri) => {
                    LastUrl = uri;
                };
                return _Instance;
            }
        }

        public override void OnCommandRecieved(string CommandName, string UserInput) {
            if(CommandName == "search") {
                Match m = RegisteredCommands["search"].Match(UserInput);
                LastUrl = m.Groups["url"].Value;
                Instance.ShowPage(m.Groups["url"].Value);
            } else if(CommandName == "resume") {
                Instance.ShowPage(LastUrl);
            }
        }



        #region Unimplemented Methods
        public override void ConfigureSettings() {
            return;
        }


        public override void OnInitialized() {
            return;
        }

        public override void OnShutdown() {
            return;
        }
        #endregion
    }
}
