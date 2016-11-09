using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ModuleAPI;
using MusicBrowser;
using Newtonsoft.Json;

namespace MusicPlayer {

    [ApplicationHook]
    public class Hook : Module {
        public override string Name { get { return "Music Browser"; } }
        public override string SemVer { get { return "0.1.0"; } }
        public override string Author { get { return "Aryan Mann"; } }
        public override Uri Website { get { return new Uri(@"http://www.aryanmann.com/"); } }

        public override Dictionary<string, Regex> RegisteredCommands {
            get {
                return new Dictionary<string, Regex>() {
                    ["Settings"] = new Regex(@"musicplayer settings", RegexOptions.IgnoreCase),
                    ["SongList"] = new Regex(@"all songs?", RegexOptions.IgnoreCase)
                };
            }
        }

        Settings SettingWindow = new Settings();
        SearchResult SearchWindow = new SearchResult();

        public override void OnInitialized() {
            Console.WriteLine($"{Name} Initialized");
        }

        public override void OnCommandRecieved(string CommandName, string UserInput) {

            switch(CommandName) {
                case "Settings":
                    if(SettingWindow == null) { SettingWindow = new Settings(); }
                    SettingWindow.Activate();
                    SettingWindow.Show();
                break;
                case "SongList":
                    if(SearchWindow == null) { SearchWindow = new SearchResult(); }
                    SearchWindow.SearchMethod = SearchResult.Method.ListAll;
                    SearchWindow.Activate();
                    SearchWindow.Show();
                break;
            }

        }

        public override void ConfigureSettings() {
            Console.WriteLine("Open Settings");
        }

        public override void OnShutdown() {
            Console.WriteLine($"{Name} Shutdown");
        }
    }

    public class Song {

        [JsonIgnore]
        public string Name {
            get {
                return Path.GetFileNameWithoutExtension(Filepath);
            }
        }
        [JsonIgnore]
        public string Filename {
            get {
                return Path.GetFileName(Filepath);
            }
        }
        [JsonIgnore]
        public string Extension {
            get {
                return Path.GetExtension(Filepath);
            }
        }

        public string Filepath { get; private set; }

        public static List<string> ValidMusicExtensions { get; private set; } = new List<string>() {
            ".mp3", ".m4a", ".ogg", ".flac"
        };
        public static List<Song> SongList { get; set; } = new List<Song>();

        public static Song GetSong(string filePath) {
            if(!File.Exists(filePath)) { return null; } else
                return new Song() { Filepath = filePath };
        }
    }

}
