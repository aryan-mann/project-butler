using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ModuleAPI;

namespace Relax {

    [ApplicationHook]
    public class Hooker : ModuleAPI.Module {

        #region Useless Stuff
        public override string Author {
            get { return "Aryan Mann"; }
        }
        public override string SemVer {
            get { return "0.1.0"; }
        }
        public override string Name {
            get { return "Relax"; }
        }
        public override Uri Website {
            get { return new Uri("http://www.aryanmann.com/"); }
        }
        public override void ConfigureSettings() {
            return;
        }
        public override void OnInitialized() {
            return;
        }
        public override void OnShutdown() {
            return;
        }
        #endregion

        public override Dictionary<string, Regex> RegisteredCommands {
            get {
                return new Dictionary<string, Regex>() {
                    ["relax"] = new Regex("(relax|calm ?down|calm|serenity now!?)")
                };
            }
        }

        public override void OnCommandRecieved(string CommandName, string UserInput) {
            if(CommandName == "relax") {
                PlayRandom();
            }
        }

        public void PlayRandom() {
            string[] validExtensions = new string[] {
                ".mp3", ".m4a"
            };

            string songPath = Path.Combine(BaseDirectory, "Songs");
            if(!Directory.Exists(songPath)) { Directory.CreateDirectory(songPath); return; }

            List<string> files = Directory.GetFiles(songPath, "*", SearchOption.AllDirectories).Where(path => validExtensions.Contains(Path.GetExtension(path).ToLower())).ToList();

            if(files.Count == 0) { return; }

            Random r = new Random();
            int fileToPlay = r.Next(0, files.Count - 1);
            Process p = Process.Start(files[fileToPlay]);
        }

    }

}
