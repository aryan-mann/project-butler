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
        /// <summary>
        /// Semantic Versioning
        /// (http://semver.org/)
        /// </summary>
        public abstract string SemVer { get; }
        public abstract string Author { get; }
        public abstract Uri Website { get; }

        // (**).CodeBase returns the location of the class library (.dll)
        // Thus, by getting it's directory, we can get BaseDirectory of the module
        public string BaseDirectory => Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(GetType()).CodeBase.Substring(8));

        public abstract Dictionary<string, Regex> RegisteredCommands { get; }

        /// <summary>
        /// When the module is first loaded/ first given a command.
        /// </summary>
        public abstract void OnInitialized();

        /// <summary>
        /// When a user input matches the Regex(s) provided by our module
        /// </summary>
        /// <param name="commandName">Which module command was detected</param>
        /// <param name="userInput">The users input which matches the command</param>
        public abstract void OnCommandRecieved(string commandName, string userInput);

        /// <summary>
        /// Opens the settings panel for the Module
        /// </summary>
        public abstract void ConfigureSettings();

        public abstract void OnShutdown();
    }
}
