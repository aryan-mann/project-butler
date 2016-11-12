using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ModuleAPI
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ApplicationHookAttribute : Attribute {
        public ApplicationHookAttribute() { }
    }
    
    public abstract class Module {

        public abstract string Name { get; }
        public abstract string SemVer { get; }
        public abstract string Author { get; }
        public abstract Uri Website { get; }
        
        public string BaseDirectory {
            get {
                return Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(GetType()).CodeBase).Substring(6);
            }
        }

        public abstract Dictionary<string, Regex> RegisteredCommands { get; }

        public abstract void OnInitialized();

        public abstract void OnCommandRecieved(string CommandName, string UserInput);
        public abstract void ConfigureSettings();

        public abstract void OnShutdown();
        
    }
}
