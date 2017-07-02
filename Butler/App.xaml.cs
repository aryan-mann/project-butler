using System;
using System.Collections.Generic;
using System.Windows;
using ModuleAPI;

namespace Butler {

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        
        public App() {
            
            //Shutdown when the main window or the last window closes
            ShutdownMode = ShutdownMode.OnMainWindowClose;

            Current.Exit += (sender, args) => {

                // Give shutdown command to all user modules
                foreach (var module in ModuleLoader.ModuleLoadOrder) {
                    module.Value.GiveOnShutdownCommand();
                }
                
                var loadSettings = Settings.LoadSettings("Preferences", AppDomain.CurrentDomain.BaseDirectory);
                if(loadSettings == null) { return; }

                Dictionary<UserModule, bool> activeStatuses = new Dictionary<UserModule, bool>();
                foreach (var userModule in ModuleLoader.ModuleLoadOrder.Values) {
                    activeStatuses.Add(userModule, userModule.Enabled);
                }

                loadSettings.Set("ActiveStatus", activeStatuses);
                loadSettings.Save();
            };

        }

    }
}
