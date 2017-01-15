using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ModuleAPI;

namespace Butler {

    public class ModuleLoader {

        //Where Modules are stored
        public static string ModuleDirectory {
            get {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Modules");
                if(!Directory.Exists(path)) { Directory.CreateDirectory(path); }

                return path;
            }
        }
        //Loaded modules and the order they are called in
        public static Dictionary<int, UserModule> ModuleLoadOrder { get; private set; } = new Dictionary<int, UserModule>();

        //Search for modules in all folders inside the Modules directory
        public static void LoadAll() {
            string[] directories = Directory.GetDirectories(ModuleDirectory, "*", SearchOption.TopDirectoryOnly);
            ModuleLoadOrder?.Clear();

            directories.ToList().ForEach(dir => {
                UserModule um = GetModuleFromDirectory(dir);
                if(um != null) {
                    int index = ModuleLoadOrder.Count + 1;
                    bool canUse = false;
                    do {
                        if(!ModuleLoadOrder.ContainsKey(index)) {
                            canUse = true;
                            ModuleLoadOrder.Add(index, um);
                        } else { index++; }
                    } while(canUse == false);
                }
            });

        }

        //Load a module given the path of the directory it is in
        private static UserModule GetModuleFromDirectory(string directory) {
            string[] files = Directory.GetFiles(directory, "*.dll", SearchOption.AllDirectories);

            UserModule mod = null;
            files.ToList().ForEach(file => {
                Assembly asm = Assembly.LoadFrom(file);

                //Get the entry class of a module:- 
                //1. Should a child of Module class
                //2. Should have ApplicationHook attribute
                List<Type> startClasses = asm.GetTypes().Where(t =>
                    (t.IsClass) && 
                    (t.BaseType == typeof(ModuleAPI.Module)) &&
                    (t.GetCustomAttribute<ApplicationHookAttribute>() != null)
                ).ToList();
                
                if(startClasses?.Count == 0) { return; }
                Type startupClass = startClasses[0];

                mod = UserModule.FromType(startupClass);
                return;
            });

            return mod ?? null;
        }
    }

}
