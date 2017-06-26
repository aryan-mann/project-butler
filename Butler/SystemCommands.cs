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

        public override async Task OnCommandRecieved(Command command) {

            switch (command.LocalCommand) {
                case "Command API":
                    if(!command.IsLocalCommand) {
                        List<ModuleInfoPacket> packetList = new List<ModuleInfoPacket>();

                        await Task.Run(() => {
                            foreach (var userModule in ModuleLoader.ModuleLoadOrder) {
                                packetList.Add(ModuleInfoPacket.FromUserModule(userModule.Value));
                            }
                        });

                        command.Respond(JsonConvert.SerializeObject(packetList)); 
                    }

                    return;
                case "Command List":
                    List<UserModule> uModules = ModuleLoader.ModuleLoadOrder.Values.ToList();
                    string output = $"{uModules.Count} Available Commands:- \n\n";

                    await Task.Run(() => {
                        foreach (var mod in uModules) {
                            output += $"> {mod.Name} ({mod.Prefix}) [{mod.RegisteredCommands.Count}]\n\n";
                            foreach (var regexes in mod.RegisteredCommands) {
                                output += $"   > {regexes.Key} => {regexes.Value.ToString()}\n";
                            }
                            output += $"\n";
                        }
                    });

                    if(!command.IsLocalCommand) {
                        command.Respond(output);
                    }

                    return;
                case "Test":
                    RemoteControlManager.Instance.InvokeOnPseudoCommandRecieved("music start radio", null);
                    break;
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
