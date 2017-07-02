using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
        public abstract string Prefix { get; }

        // (**).CodeBase returns the location of the class library (.dll)
        // Thus, by getting it's directory, we can get BaseDirectory of the module
        public string ModuleDirectory => Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(GetType()).CodeBase.Substring(8));

        // TURN THIS INTO AN ORDERED DICTIONARY GOD DAMN IT
        public abstract Dictionary<string, Regex> RegisteredCommands { get; }

        /// <summary>
        /// When the module is first loaded/ first given a command.
        /// </summary>
        public abstract Task OnInitialized();

        /// <summary>
        /// When a user input matches the Regex(s) provided by our module
        /// </summary>
        /// <param name="cmd">The command recieved</param>
        public abstract Task OnCommandRecieved(Command cmd);

        /// <summary>
        /// Opens the settings panel for the Module
        /// </summary>
        public abstract Task ConfigureSettings();

        /// <summary>
        /// When the main application is shutting down
        /// </summary>
        /// <returns></returns>
        public abstract Task OnShutdown();
    }

    public static class ModuleExtensions {
        public static string GetBaseDirectoryOfType(Type t) => Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(t).CodeBase.Substring(8));
        public static string GetBaseDirectory(this Module m) => Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(m.GetType()).CodeBase.Substring(8));
    }

}
