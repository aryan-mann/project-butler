using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Security.RightsManagement;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows;
using ModuleAPI;

namespace Browser {

    [ApplicationHook]
    public class Hook: Module {

        #region Properties
        public override string Author => "Aryan Mann";
        public override string Name => "Simple Browser";
        public override string Prefix => "brws";
        
        public override Dictionary<string, Regex> RegisteredCommands => new Dictionary<string, Regex>() {
            ["search"] = new Regex(@"^search (?<url>.+)$"),
            ["resume"] = new Regex(@"^resume$"),
            ["close"] = new Regex(@"^((min(imize)?)|close)"),
            ["special search"] = new Regex(@"^(?<provider>.+)( )?=>( )?(?<query>.+)$")
        };

        public override string SemVer => "0.1.0";
        public override Uri Website => new Uri("http://www.aryanmann.com/");
        #endregion

        //For resuming browsing.
        public string LastUrl = "";

        WebBrowser _instance = new WebBrowser();
        WebBrowser Instance {
            get {
                if(!_instance.IsLoaded) {
                    _instance = new WebBrowser();
                }
                _instance.Navigated += (uri) => {
                    LastUrl = uri;
                };
                return _instance;
            }
        }

        //Uses {0} to denote query string
        public Dictionary<string, string> UrlConstructorDictionary => new Dictionary<string, string> {
            ["stack overflow"] = @"stackoverflow.com/search?q={0}",
            ["youtube"] = @"youtube.com/results?search_query={0}",
            ["wikipedia"] = @"https://en.wikipedia.org/w/index.php?search={0]"
        };

        public override void OnCommandRecieved(Command cmd) {
            
            if(cmd.LocalCommand == "search") {
                Match m = RegisteredCommands[cmd.LocalCommand].Match(cmd.UserInput);
                LastUrl = m.Groups["url"].Value;
                Instance.ShowPage(m.Groups["url"].Value);
            }

            if(cmd.LocalCommand == "resume") {
                Instance.ShowPage(LastUrl);
            }

            if (cmd.LocalCommand == "close") {
                Instance.Hide();
            }

            if (cmd.LocalCommand == "special search") {
                Match m = RegisteredCommands[cmd.LocalCommand].Match(cmd.UserInput);

                string provider = m.Groups["provider"].Value.ToLower().Trim();
                string query = HttpUtility.HtmlEncode(m.Groups["query"].Value.ToLower());

                foreach (var kvp in UrlConstructorDictionary) {
                    if (kvp.Key.Equals(provider)) {
                        Instance.ShowPage($@"http://www.{string.Format(kvp.Value, query)}");
                        break;
                    }
                }
            }
        }
        
        #region Unimplemented Methods
        public override void ConfigureSettings() { }
        public override void OnInitialized() { }
        public override void OnShutdown() { }
        #endregion
    }
}
