using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ModuleAPI
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class ApplicationHookAttribute : Attribute { }
    
    public abstract class Module {

        public abstract string Name { get; }
        public abstract string SemVer { get; }
        public abstract string Author { get; }
        public abstract Uri Website { get; }

        // (**).CodeBase returns the location of the class library (.dll)
        // Thus, by getting it's directory, we can get BaseDirectory of the module
        public string BaseDirectory => Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(GetType()).CodeBase.Substring(8));

        public abstract Dictionary<string, Regex> RegisteredCommands { get; }

        public abstract void OnInitialized();

        public abstract void OnCommandRecieved(string commandName, string userInput);
        public abstract void ConfigureSettings();

        public abstract void OnShutdown();
    }
}
