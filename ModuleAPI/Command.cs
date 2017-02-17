using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ModuleAPI {

    public class Command {

        /// <summary>
        /// The regular expression that matched with the user input
        /// </summary>
        public Regex CommandMatcher { get; }
        /// <summary>
        /// The user input
        /// </summary>
        public string UserInput { get; }
        /// <summary>
        /// Which command was recieved (in local context)
        /// </summary>
        public string LocalCommand { get; }

        public TcpClient Client { get; }
        public bool IsLocalCommand => Client == null;

        public Command(Regex commandMatcher, string userInput, string localCommand, TcpClient client = null) {
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
            if (string.IsNullOrWhiteSpace(text) || IsLocalCommand) { return; }

            if (!Client.Connected) {
                Client.Connect((IPEndPoint) Client.Client.RemoteEndPoint);
            }
            
            byte[] message = Encoding.UTF8.GetBytes($"\n{text}\n");
            ((NetworkStream)Client.GetStream()).Write(message, 0, message.Length);
        }
    }

}
