using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ModuleAPI;

namespace Browser {

    [ApplicationHook]
    public class Hook: Module {

        #region Properties
        public override string Author => "Aryan Mann";
        public override string Name => "Simple Browser";

        public override Dictionary<string, Regex> RegisteredCommands => new Dictionary<string, Regex>() {
            ["search"] = new Regex(@"^search (?<url>.+)$"),
            ["resume"] = new Regex(@"^sr$")
        };

        public override string SemVer => "0.1.0";
        public override Uri Website => new Uri("http://www.aryanmann.com/");
        #endregion

        //For resuming browsing.
        string _lastUrl = "";

        WebBrowser _instance = new WebBrowser();
        WebBrowser Instance {
            get {
                if(!_instance.IsLoaded) {
                    _instance = new WebBrowser();
                }
                _instance.Navigated += (uri) => {
                    _lastUrl = uri;
                };
                return _instance;
            }
        }

        public override void OnCommandRecieved(string commandName, string userInput) {
            if(commandName == "search") {
                Match m = RegisteredCommands["search"].Match(userInput);
                _lastUrl = m.Groups["url"].Value;
                Instance.ShowPage(m.Groups["url"].Value);
            } else if(commandName == "resume") {
                Instance.ShowPage(_lastUrl);
            }
        }



        #region Unimplemented Methods
        public override void ConfigureSettings() { }

        public override void OnInitialized() { }

        public override void OnShutdown() { }
        #endregion
    }
}
