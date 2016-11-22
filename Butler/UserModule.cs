using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Butler {

    public class UserModule {

        //---> INSTANCE MANAGEMENT <---//
        public Type ModuleType { get; private set; }
        private object _Instance;
        public object Instance {
            get {
                if(ModuleType == null) { return null; }
                if(_Instance != null) { return _Instance; }

                _Instance = Activator.CreateInstance(ModuleType);
                return _Instance;
            }
        }
        public bool Enabled { get; set; } = true;

        //--> PROPERTIES <--//
        private string _Name;
        public string Name {
            get {
                if(!string.IsNullOrWhiteSpace(_Name)) { return _Name; }

                _Name = GetPropertyValue<string>("Name") ?? "";
                return _Name;
            }
        }

        private string _SemVer;
        public string SemVer {
            get {
                if(!string.IsNullOrWhiteSpace(_SemVer)) { return _SemVer; }

                _SemVer = GetPropertyValue<string>("SemVer") ?? "0.1.0";
                return _SemVer;
            }
        }

        private string _Author;
        public string Author {
            get {
                if(!string.IsNullOrWhiteSpace(_Author)) { return _Author; }

                _Author = GetPropertyValue<string>("Author") ?? "";
                return _Author;
            }
        }

        private Uri _Website;
        public Uri Website {
            get {
                if(_Website != null) { return _Website; }

                _Website = GetPropertyValue<Uri>("Website");
                return _Website;
            }
        }

        public string _BaseDirectory;
        public string BaseDirectory {
            get {
                if(!string.IsNullOrWhiteSpace(_BaseDirectory)) { return _BaseDirectory; }
                _BaseDirectory = GetPropertyValue<string>("BaseDirectory");
                return _BaseDirectory;
            }
        }

        private Dictionary<string, Regex> _RegisteredCommands;
        public Dictionary<string, Regex> RegisteredCommands {
            get {
                if(_RegisteredCommands != null) { return _RegisteredCommands; }

                _RegisteredCommands = GetPropertyValue<Dictionary<string, Regex>>("RegisteredCommands");
                return _RegisteredCommands;
            }
        }

        private string _CommandStrings;
        public string CommandStrings {
            get {
                if(_CommandStrings != null) { return _CommandStrings; }
                string joined = "";
                foreach(Regex r in RegisteredCommands.Values.ToList()) {
                    joined += " | [ " + r.ToString() + " ]";
                }
                _CommandStrings = joined.Substring(3);
                return _CommandStrings;
            }
        }

        private MethodInfo OnCommandRecievedMethod;

        private UserModule(Type t) {
            ModuleType = t;

            OnCommandRecievedMethod = ModuleType.GetMethod("OnCommandRecieved");
        }

        public void GiveRegexCommand(string _key, string _query) {
            if(!RegisteredCommands.Keys.Contains(_key)) { return; }
            
            OnCommandRecievedMethod.Invoke(Instance, new object[] {
                _key, _query
            });
        }

        private T GetPropertyValue<T>(string name) {
            PropertyInfo pInfo = ModuleType.GetProperty(name);
            if(pInfo == null) { return default(T); }

            try {
                object ob = pInfo.GetValue(Instance);
                if(ob == null) { return default(T); } else { return (T)ob; }
            } catch { return default(T); }

        }
        //----------------------------------------------------->

        public static Dictionary<Type, object> InstanceCollection { get; private set; } = new Dictionary<Type, object>();

        public static UserModule FromType(Type t) {
            return new UserModule(t);
        }
        public static Task<UserModule> FromTypeAsync(Type t) {
            return new Task<UserModule>(()=>FromType(t));
        }

    }

}
