using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Butler {

    public static class TaskbarIconManager {

        private static NotifyIcon _Instance;
        public static NotifyIcon Instance {
            get {
                if(_Instance == null) {
                    _Instance = new NotifyIcon() {
                        Visible = true,
                        Icon = SystemIcons.Application,
                        Text = "Text",
                        BalloonTipIcon = ToolTipIcon.Info,
                        BalloonTipText = "Balloon Text",
                        BalloonTipTitle = "Balloon Title",
                        ContextMenuStrip = DefaultContextMenuStrip
                    };
                }
                return _Instance;
            }
        }

        private static ContextMenuStrip DefaultContextMenuStrip {
            get {
                return new ContextMenuStrip() {
                    AutoSize = true,
                    ShowCheckMargin = false,
                    ShowImageMargin = false,
                    LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow
                };
            }
        }

        public static Dictionary<string, Action> MenuItems = new Dictionary<string, Action>();
        
        public static bool AddItem(string name, Action function) {
            if(!MenuItems.ContainsKey(name)) {
                MenuItems.Add(name, function);
                return true;
            } else {
                return false;
            }
        }
        public static void RemoveItem(string name) {
            if(MenuItems.ContainsKey(name)) {
                MenuItems.Remove(name);
            }
        }
        public static void CommitItems() {
            if(Instance.ContextMenuStrip == null) {
                Instance.ContextMenuStrip = DefaultContextMenuStrip;
            }

            Instance.ContextMenuStrip.Items.Clear();
            foreach(var kvp in MenuItems) {
                ToolStripButton tsb = new ToolStripButton();
                tsb.Text = kvp.Key.ToString();
                tsb.Click += (sender, e) => {
                    kvp.Value();
                };
                Instance.ContextMenuStrip.Items.Add(tsb);
            }
        }

        public static void SetActive(bool b) { Instance.Visible = b; }

        public static void Dispose() {
            Instance.Dispose();
        }
        
        static TaskbarIconManager() {
            SetActive(false);
        }
    }

}
