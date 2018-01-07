using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Butler.Core;
using ModuleAPI;
using Newtonsoft.Json;

namespace Butler.InbuiltModules {

    public class SystemCommands: Module {

        public override string Name => "System";
        public override string SemVer => "0.1.0";
        public override string Author => "Butler";
        public override Uri Website => new Uri(@"http://www.butler.aryanmann.com/");
        public override string Prefix => "--";

        public override Dictionary<string, Regex> RegisteredCommands => new Dictionary<string, Regex>() {
            ["Command List"] = new Regex(@"command list"),
            ["Command API"] = new Regex(@"command api"),
            ["Settings"] = new Regex(@"^config (?<prefix>.+)$")
        };

        public override async Task OnCommandRecieved(Command cmd) {

            if(cmd.LocalCommand == "Command API") {
                if(cmd.IsLocalCommand)
                    return;

                var packetList = new List<ModuleInfoPacket>();

                await Task.Run(() => {
                    foreach(var userModule in ModuleLoader.ModuleLoadOrder) {
                        packetList.Add(ModuleInfoPacket.FromUserModule(userModule.Value));
                    }
                });

                cmd.Respond(JsonConvert.SerializeObject(packetList));

                return;
            }

            if(cmd.LocalCommand == "Command List") {
                var uModules = ModuleLoader.ModuleLoadOrder.Values.ToList();
                string output = $"{uModules.Count} Available Commands:- \n\n";

                await Task.Run(() => {
                    foreach(var mod in uModules) {
                        output += $"> {mod.Name} ({mod.Prefix}) [{mod.RegisteredCommands.Count}]\n\n";
                        foreach(var regexes in mod.RegisteredCommands) {
                            output += $"   > {regexes.Key} => {regexes.Value.ToString()}\n";
                        }
                        output += $"\n";
                    }
                });

                if(!cmd.IsLocalCommand) {
                    cmd.Respond(output);
                }

                return;
            }

            if(cmd.LocalCommand == "Settings") {
                var prefix = RegisteredCommands[cmd.LocalCommand].Match(cmd.UserInput).Groups["prefix"].Value.Trim();
                var module = ModuleLoader.ModuleLoadOrder.Values.FirstOrDefault(v => v.Prefix == prefix);
                if(module == null) { return; }

                module.GiveConfigureSettingsCommand();
            }
        }

        public override async Task OnInitialized() => await Task.CompletedTask;
        public override async Task ConfigureSettings() => await Task.CompletedTask;
        public override async Task OnShutdown() => await Task.CompletedTask;
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
