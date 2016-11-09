using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Butler {

    public static class SystemModules {
        public static Dictionary<string, Regex> Commands { get; set; } = new Dictionary<string, Regex>() {
            ["CommandList"] = new Regex(@"list commands", RegexOptions.IgnoreCase)
        };

        static Results _ResInstance = new Results();
        static Results ResInstance {
            get {
                if(_ResInstance == null) { _ResInstance = new Results(); }
                return _ResInstance;
            }
            set {
                _ResInstance = value ?? new Results();
            }
        }

        public static void InitiateCommand(string _CommandName, string _Query) {
            if(!Commands.Keys.Contains(_CommandName)) { return; }

            ResInstance = new Results();

            switch(_CommandName) {
                case "CommandList":
                    List<string> _Commands = new List<string>();
                    foreach(UserModule um in ModuleLoader.ModuleLoadOrder.Values) {
                        foreach(var kvp in um.RegisteredCommands) {
                            _Commands.Add($@"[{um.Name}] : {kvp.Key} -> {kvp.Value.ToString()}");
                        }
                    }
                    ResInstance.SetupData<string>(_Commands);
                    ResInstance.Show();
                    break;
            }
        }
    }

}
