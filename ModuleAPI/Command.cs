using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace ModuleAPI {

    public class Command {

        public string UserModuleName { get; } = string.Empty;
        /// <summary>
        /// The regular expression that matched with the user input
        /// </summary>
        public Regex CommandMatcher { get; } = new Regex("");
        /// <summary>
        /// The user input
        /// </summary>
        public string UserInput { get; } = string.Empty;
        /// <summary>
        /// Which command was recieved (in local context)
        /// </summary>
        public string LocalCommand { get; } = string.Empty;

        public TcpClient Client { get; }
        public bool IsLocalCommand => Client == null;

        public static Command Empty { get; } = new Command();
        
        public delegate void OnResponded(string response, Command com, TcpClient client = null);
        public static OnResponded Responded { get; set; }

        private Command() { }
        public Command(string userModuleName, Regex commandMatcher, string userInput, string localCommand, TcpClient client = null) {
            UserModuleName = userModuleName;
            UserInput = userInput;
            CommandMatcher = commandMatcher;
            LocalCommand = localCommand;
            Client = client;
        }

        /// <summary>
        /// A collection of named capturing groups
        /// </summary>
        public GroupCollection Matches => CommandMatcher.Match(UserInput).Groups;
        
        public void Respond(string text) {
            if (string.IsNullOrWhiteSpace(text)) { return; }

            if (IsLocalCommand) {
                Responded?.Invoke(text, this, null);
                return;
            }

            if (!Client.Connected) {
                Client.Connect((IPEndPoint) Client.Client.RemoteEndPoint);
            }
            
            var message = Encoding.UTF8.GetBytes($"\n{text}\n");
            Client.GetStream().Write(message, 0, message.Length);
            Responded?.Invoke(text, this, Client);
        }

        public delegate void OnPseudoCommand(string command, TcpClient client);
        public static event OnPseudoCommand PseudoCommand;

        public static void InvokePseudoCommand(string command, TcpClient client) => PseudoCommand?.Invoke(command, client);
        public static void GiveGeneralResponse(string response, Command com = null) => Responded?.Invoke(response, com ?? Command.Empty, null);

        public override string ToString() => $"{UserModuleName} {UserInput}";
    }

}
