using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ModuleAPI {

    public class Settings {

        public static JsonSerializerSettings SerializerSettings { get; } = new JsonSerializerSettings() {
            TypeNameHandling = TypeNameHandling.Auto
        };

        public string Name { get; private set; }
        private Dictionary<string, object> _data = new Dictionary<string, object>();

        public string SavePath { get; private set; }

        private Settings() { }

        public static Settings CreateSettings(string name, string saveDirectory) {
            if(!Directory.Exists(saveDirectory)) { return null; }

            string filePath = Path.Combine(saveDirectory, $"{name}.json");
            if(File.Exists(filePath)) { return null; }

            try {
                File.Create(filePath);
            } catch { return null; }

            return new Settings() {
                Name = name,
                SavePath = filePath
            };
        }
        public static Settings LoadSettings(string name, string saveDirectory) {
            string filePath = Path.Combine(saveDirectory, $"{name}.json");
            if(!File.Exists(filePath)) { return null; }

            return new Settings() {
                Name = name,
                SavePath = filePath
            };
        }
        public static Settings CreateOrLoadSettings(string name, string saveDirectory) {
            string filePath = Path.Combine(saveDirectory, $"{name}.json");

            if (!File.Exists(filePath)) {
                try {
                    File.Create(filePath);
                } catch { return null; }
            }

            return new Settings() {
                Name = name,
                SavePath = filePath
            };

        }

        public bool Save() {
            if(!File.Exists(SavePath)) { return false; }

            try {
                File.WriteAllText(SavePath, JsonConvert.SerializeObject(_data), Encoding.UTF8);
                return true;
            } catch { return false; }
        }
        public bool Load() {
            if(!File.Exists(SavePath)) { return false; }

            try {

                var data = string.Empty;
                
                WaitForFileAccess(SavePath, () => {
                    data = File.ReadAllText(SavePath);
                });
                
                _data = JsonConvert.DeserializeObject<Dictionary<string, object>>(data) ?? new Dictionary<string, object>();
                return true;
            } catch(Exception exc) { return false; }
        }

        public void Set(string name, object obj) => _data[name] = obj;
        public object Get(string name) => _data.ContainsKey(name) ? _data[name] : null;
        public T Get<T>(string name) {
            if (!_data.ContainsKey(name)) return default(T);

            var t = _data[name].GetType();

            if(_data[name] is T) { return (T) _data[name]; }
            return default(T);
        }
        public bool Remove(string name) => _data.Remove(name);

        public static void WaitForFileAccess(string filepath, Action act) {
            if(!File.Exists(filepath)) { act(); }

            loopStart: 
            try {
                using (File.Open(filepath, FileMode.Open, FileAccess.ReadWrite)) { }
                act();
            } catch {
                Task.Delay(250).Wait();
                goto loopStart;
            }
        }
    }

}
