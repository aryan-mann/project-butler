using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ModuleAPI;

namespace Butler {

    public static class ModuleLoader {

        /// <summary>
        /// Where all the Modules are located
        /// </summary>
        public static string ModuleDirectory {
            get {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Modules");
                if(!Directory.Exists(path)) { Directory.CreateDirectory(path); }

                return path;
            }
        }
        
        /// <summary>
        /// Loaded Modules and the order they are loaded in
        /// </summary>
        public static Dictionary<int, UserModule> ModuleLoadOrder { get; private set; } = new Dictionary<int, UserModule>();

        public delegate void OnLoadingStarted(int moduleCount);
        public static event OnLoadingStarted LoadingStarted;

        public delegate void OnModuleLoaded(UserModule module);
        public static event OnModuleLoaded ModuleLoaded;

        public delegate void OnLoadingEnded();
        public static event OnLoadingEnded LoadingEnded;


        /// <summary>
        /// Load all modules in the Module Directory
        /// </summary>
        public static void LoadAll() {

            ModuleLoadOrder = ModuleLoadOrder ?? new Dictionary<int, UserModule>();
            ModuleLoadOrder?.Clear();

            UserModule sysModule = UserModule.FromType(typeof(SystemCommands));
            ModuleLoadOrder.Add(ModuleLoadOrder.Count, sysModule);

            string[] directories = Directory.GetDirectories(ModuleDirectory, "*", SearchOption.TopDirectoryOnly);
            LoadingStarted?.Invoke(directories.Length);
            
            // Search through all directories and add user modules
            directories.ToList().ForEach(dir => {
                UserModule um = GetModuleFromDirectory(dir);

                // If a module is detected, add it to the ModuleLoadOrder dictionary
                if(um != null) {
                    int index = ModuleLoadOrder.Count + 1;
                    bool canUse = false;
                    do {
                        if(!ModuleLoadOrder.ContainsKey(index)) {
                            canUse = true;
                            ModuleLoadOrder.Add(index, um);
                            ModuleLoaded?.Invoke(um);
                        } else { index++; }
                    } while(canUse == false);
                }
                
            });

            LoadingEnded?.Invoke();
        }
        public static async Task LoadAllAsync() {
            ModuleLoadOrder = ModuleLoadOrder ?? new Dictionary<int, UserModule>();
            ModuleLoadOrder?.Clear();

            UserModule sysModule = await UserModule.FromTypeAsync(typeof(SystemCommands));
            ModuleLoadOrder.Add(ModuleLoadOrder.Count, sysModule);

            string[] directories = Directory.GetDirectories(ModuleDirectory, "*", SearchOption.TopDirectoryOnly);
            LoadingStarted?.Invoke(directories.Length);

            List<UserModule> foundModules = await GetModulesFromDirectoriesAsync(directories);
            foundModules.ForEach(um => {
                int index = ModuleLoadOrder.Count + 1;
                bool canUse = false;

                do {
                    if (!ModuleLoadOrder.ContainsKey(index)) {
                        canUse = true;
                        ModuleLoadOrder.Add(index, um);
                        ModuleLoaded?.Invoke(um);
                    }
                    else {
                        index++;
                    }
                } while (canUse == false);

            });

            LoadingEnded?.Invoke();
        }

        /// <summary>
        /// Load a module anywhere inside a specific directory
        /// </summary>
        /// <param name="directory">Location of the module</param>
        /// <returns></returns>
        private static UserModule GetModuleFromDirectory(string directory) {
            IEnumerable<string> files = Directory.EnumerateFiles(directory, "*.dll", SearchOption.AllDirectories);

            UserModule mod = null;
            // Load each *.dll file found in the directory
            files.ToList().ForEach(async file => {
                Assembly asm = Assembly.LoadFrom(file);

                // Search for a type in the loaded assembly that:- 
                // 1. Is a child of the Module class
                // 2. Has the ApplicationHook attribute (that can only be applied to one class)

                List<Type> startClasses = asm.GetTypes().Where(t =>
                    t.IsClass &&
                    t.BaseType == typeof(ModuleAPI.Module) &&
                    t.GetCustomAttribute<ApplicationHookAttribute>() != null
                ).ToList();

                // If any class meeting these conditions is found,
                // Store that as the 'Startup Class' i.e. the class
                // which will recieve commands through Project Butler
                if(startClasses.Count > 0) {
                    Type startupClass = startClasses[0];
                    mod = await UserModule.FromTypeAsync(startupClass);
                }
            });
            
            return mod;
        }
        private static async Task<List<UserModule>> GetModulesFromDirectoriesAsync(string[] directories) {

            IEnumerable<Task<UserModule>> modTasks = directories.Where(dir => !dir.EndsWith("...ignore")).Select(async dir => {
                string[] files = Directory.GetFiles(dir, "*.dll", SearchOption.AllDirectories);
                UserModule mod = null;

                foreach (string file in files) {
                    Assembly asm = Assembly.LoadFrom(file);

                    if(mod != null) { goto foundMod; }

                    List<Type> startClasses = asm.GetTypes().Where(t =>
                        t.IsClass &&
                        t.BaseType == typeof(ModuleAPI.Module) &&
                        t.GetCustomAttribute<ApplicationHookAttribute>() != null
                    ).ToList();

                    if(startClasses.Count <= 0) { continue; }

                    Type startupClass = startClasses[0];
                    mod = await UserModule.FromTypeAsync(startupClass);
                }

                foundMod:
                    return mod;
            });

            List<UserModule> foundModules = new List<UserModule>();
            foreach (Task<UserModule> modTask in modTasks) {
                UserModule um = await modTask;
                if(um != null)
                    foundModules.Add(um);
            }

            return foundModules;
        }

    }

}
