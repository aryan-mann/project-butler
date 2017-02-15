using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ModuleAPI;

namespace Butler {

    public class UserModule {

        /* Instance Management */
        public Type ModuleType { get; private set; }
        private object _instance;
        public object Instance {
            get {
                if(ModuleType == null) { return null; }
                if(_instance != null) { return _instance; }

                _instance = Activator.CreateInstance(ModuleType);
                return _instance;
            }
        }
        public bool Enabled { get; set; } = true;

        /* Module Properties */
        private string _name;
        public string Name {
            get {
                if(!string.IsNullOrWhiteSpace(_name)) { return _name; }

                _name = GetPropertyValue<string>("Name") ?? "";
                return _name;
            }
        }

        private string _semVer;
        public string SemVer {
            get {
                if(!string.IsNullOrWhiteSpace(_semVer)) { return _semVer; }

                _semVer = GetPropertyValue<string>("SemVer") ?? "0.1.0";
                return _semVer;
            }
        }

        private string _author;
        public string Author {
            get {
                if(!string.IsNullOrWhiteSpace(_author)) { return _author; }

                _author = GetPropertyValue<string>("Author") ?? "";
                return _author;
            }
        }

        private string _prefix;
        public string Prefix {
            get {
                if(!string.IsNullOrWhiteSpace(_prefix)) { return _prefix; }

                _prefix = GetPropertyValue<string>("Prefix") ?? "";
                return _prefix;
            }
        }

        private Uri _website;
        public Uri Website {
            get {
                if(_website != null) { return _website; }

                _website = GetPropertyValue<Uri>("Website");
                return _website;
            }
        }

        private string _baseDirectory;
        public string BaseDirectory {
            get {
                if(!string.IsNullOrWhiteSpace(_baseDirectory)) { return _baseDirectory; }
                _baseDirectory = GetPropertyValue<string>("BaseDirectory");
                return _baseDirectory;
            }
        }

        private Dictionary<string, Regex> _registeredCommands;
        public Dictionary<string, Regex> RegisteredCommands {
            get {
                if(_registeredCommands != null) { return _registeredCommands; }

                _registeredCommands = GetPropertyValue<Dictionary<string, Regex>>("RegisteredCommands");
                return _registeredCommands;
            }
        }

        /// <summary>
        /// Invoke commands more easily by caching information on this method
        /// </summary>
        private readonly MethodInfo _onCommandRecievedMethod;

        private UserModule(Type t) {
            ModuleType = t;
            _onCommandRecievedMethod = ModuleType.GetMethod("OnCommandRecieved");
        }

        //Indicate to the Module that a command has been received for it 
        public void GiveRegexCommand(Command cmd) {
            if(!RegisteredCommands.Keys.Contains(cmd.LocalCommand)) { return; }

            _onCommandRecievedMethod.Invoke(Instance, new object[] {
                cmd
            });
        }

        // Get the value of a property of the ApplicationHook class of the modile
        /// <summary>
        /// Try and get the value of a property of a module
        /// </summary>
        /// <typeparam name="T">Datatype of the value</typeparam>
        /// <param name="name">Name of the value</param>
        /// <returns></returns>
        private T GetPropertyValue<T>(string name) {
            PropertyInfo pInfo = ModuleType.GetProperty(name);
            if(pInfo == null) { return default(T); }

            try {
                object ob = pInfo.GetValue(Instance);
                if(ob == null) { return default(T); } else { return (T) ob; }
            } catch { return default(T); }

        }

        /* STATIC METHODS */

        /// <summary>
        /// If a module accepts user input, give it the command
        /// </summary>
        /// <param name="query">User input</param>
        /// <returns></returns>
        public static bool FindAndGiveRegexCommand(string query) {

            if(string.IsNullOrWhiteSpace(query)) {
                return false;
            }

            UserModule selectedModule = null;
            string selectedRegexKey = "";
            bool matchFound = false;

            Match m = Regex.Match(query, "^(?<first>.+?)\\s+(?<rest>.+)$");
            string firstWord = m.Groups["first"].Value;
            string rest = m.Groups["rest"].Value;

            UserModule shortListModule = ModuleLoader.ModuleLoadOrder.Values.FirstOrDefault(val => val.Prefix == firstWord);
            if(shortListModule != null) {
                foreach(var rgxs in shortListModule.RegisteredCommands) {
                    if(matchFound) { break; }

                    if(rgxs.Value.Match(rest).Success) {
                        selectedRegexKey = rgxs.Key;
                        selectedModule = shortListModule;
                        matchFound = true;
                        break;
                    }
                }
            }

            //If a match is not found or the user input is invalid, select all user input text
            if(!matchFound || selectedModule == null || string.IsNullOrWhiteSpace(selectedRegexKey)) {
                return false;
            }

            selectedModule.GiveRegexCommand(new Command(shortListModule.RegisteredCommands[selectedRegexKey], rest, selectedRegexKey));
            return true;
        }

        /// <summary>
        /// A dictionary of all the Startup Classes of all modules and their instances
        /// </summary>
        public static Dictionary<Type, object> InstanceCollection { get; private set; } = new Dictionary<Type, object>();

        public static UserModule FromType(Type t) {
            return new UserModule(t);
        }
        public static async Task<UserModule> FromTypeAsync(Type t) {
            Task<UserModule> mod = new Task<UserModule>(() => FromType(t));
            await mod;

            return mod.Result;
        }
    }

}
