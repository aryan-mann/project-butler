using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using ModuleAPI;
using Newtonsoft.Json;

namespace Butler {

    public class SystemCommands: Module {

        public override string Name { get; } = "System";
        public override string SemVer { get; } = "0.1.0";
        public override string Author { get; } = "Butler";
        public override Uri Website { get; } = new Uri(@"http://www.butler.aryanmann.com/");
        public override string Prefix { get; } = "system";

        public override Dictionary<string, Regex> RegisteredCommands { get; } = new Dictionary<string, Regex>() {
            ["Command List"] = new Regex(@"command list"),
            ["Command API"] = new Regex(@"command api")
        };

        public override void OnCommandRecieved(Command command) {

            if(command.LocalCommand == "Command API") {
                if(!command.IsLocalCommand) {
                    command.Respond(JsonConvert.SerializeObject(ModuleLoader.ModuleLoadOrder.Values.ToList()));
                }
            }

            if(command.LocalCommand == "Command List") {
                List<UserModule> uModules = ModuleLoader.ModuleLoadOrder.Values.ToList();
                string output = $"{uModules.Count} Available Commands:- \n\n";

                foreach (var mod in uModules) {
                    output += $"> {mod.Name} ({mod.Prefix}) [{mod.RegisteredCommands.Count}]\n\n";
                    foreach (var regexes in mod.RegisteredCommands) {
                        output += $"   > {regexes.Key} => {regexes.Value.ToString()}\n";
                    }
                    output += $"\n";
                }

                if(!command.IsLocalCommand) {
                    command.Respond(output);
                }
            }

        }

        public override void OnInitialized() { }
        public override void ConfigureSettings() { }
        public override void OnShutdown() { }
    }

}
