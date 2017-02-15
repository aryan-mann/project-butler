using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        public IPEndPoint RequesterIP { get; }
        public bool IsLocalCommand => RequesterIP == null;

        public Command(Regex commandMatcher, string userInput, string localCommand, IPEndPoint ip = null) {
            UserInput = userInput;
            CommandMatcher = commandMatcher;
            LocalCommand = localCommand;
            RequesterIP = ip;
        }

        /// <summary>
        /// A collection of named capturing groups
        /// </summary>
        public GroupCollection Matches => CommandMatcher.Match(UserInput).Groups;
    }

}
