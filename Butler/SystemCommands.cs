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

        public override string Name => "System";
        public override string SemVer => "0.1.0";
        public override string Author => "Butler";
        public override Uri Website => new Uri(@"http://www.butler.aryanmann.com/");
        public override string Prefix => "system";

        public override Dictionary<string, Regex> RegisteredCommands => new Dictionary<string, Regex>() {
            ["Command List"] = new Regex(@"command list"),
            ["Command API"] = new Regex(@"command api"),
            ["Test"] = new Regex(@"Test", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace)
        };

        public override void OnCommandRecieved(Command command) {

            if(command.LocalCommand == "Command API") {
                if(!command.IsLocalCommand) {
                    List<ModuleInfoPacket> packetList = new List<ModuleInfoPacket>();
                    foreach (var userModule in ModuleLoader.ModuleLoadOrder) {
                        packetList.Add(ModuleInfoPacket.FromUserModule(userModule.Value));
                    }

                    command.Respond(JsonConvert.SerializeObject(packetList)); 
                }

                return;
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

                return;
            }

            if (command.LocalCommand == "Test") {
                RemoteControlManager.InvokeOnPseudoCommandRecieved("music start radio", null);
            }

        }

        public override void OnInitialized() { }
        public override void ConfigureSettings() { }
        public override void OnShutdown() { }
    }

    // Representation of all commands of all modules that external apps will recognize
    public class ModuleInfoPacket {
        public string Name;
        public string Prefix;
        public Dictionary<string, Regex> Commands;

        private ModuleInfoPacket() { }

        public static ModuleInfoPacket FromUserModule(UserModule mod) {
            return new ModuleInfoPacket() {
                Name = mod.Name,
                Prefix = mod.Prefix,
                Commands = mod.RegisteredCommands
            };
        }
    }

}
