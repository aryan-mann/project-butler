using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Butler.Properties;

namespace Butler {

    //How the taskbar icon is rendered
    public class DarkToolStripRenderer: ToolStripRenderer {

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e) {
            base.OnRenderToolStripBorder(e);
            ControlPaint.DrawBorder(e.Graphics, e.AffectedBounds, Color.FromArgb(51, 171, 210), ButtonBorderStyle.Solid);
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e) {
            base.OnRenderToolStripBackground(e);
            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(35, 36, 37)), e.ToolStrip.ClientRectangle);
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e) {
            base.OnRenderMenuItemBackground(e);
            e.Graphics.FillRectangle(new SolidBrush(e.Item.BackColor), e.ToolStrip.ClientRectangle);
        }

    }

    public static class TaskbarIconManager {

        //NotifyIcon = Taskbar Icon
        private static NotifyIcon _instance;
        public static NotifyIcon Instance {
            get {
                _instance = _instance ?? new NotifyIcon() {
                    Visible = true,
                    Icon = Icon.FromHandle(Resources.ButlerLogo2.GetHicon()),
                    Text = @"Text",
                    BalloonTipIcon = ToolTipIcon.Info,
                    BalloonTipText = @"Balloon Text",
                    BalloonTipTitle = @"Balloon Title",
                    ContextMenuStrip = DefaultContextMenuStrip
                };
                return _instance;
            }
        }

        //The foundation for our custom context menu
        private static ContextMenuStrip DefaultContextMenuStrip => new ContextMenuStrip() {
            ShowCheckMargin = false,
            ShowImageMargin = false,
            AutoSize = true,
            LayoutStyle = ToolStripLayoutStyle.Flow,
            Renderer = new DarkToolStripRenderer()
        };

        //List of menu items inside the taskbar context menu
        public static Dictionary<string, Action> MenuItems = new Dictionary<string, Action>();

        //Adding and removing items is not reflected until commit items is called
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
                //Initialize Button
                ToolStripMenuItem tsb = new ToolStripMenuItem() {
                    Text = kvp.Key,
                    TextAlign = ContentAlignment.MiddleCenter,
                    DisplayStyle = ToolStripItemDisplayStyle.Text,
                    BackColor = Color.FromArgb(35, 36, 37),
                    ForeColor = Color.FromArgb(241, 241, 241)
                };
                //Events
                tsb.Click += (sender, e) => {
                    kvp.Value();
                };

                tsb.MouseEnter += (sender, e) => {
                    tsb.BackColor = Color.FromArgb(77, 78, 79);
                };
                tsb.MouseLeave += (sender, e) => {
                    tsb.BackColor = Color.FromArgb(35, 36, 37);
                };

                //Add To Context Menu
                Instance.ContextMenuStrip.Items.Add(tsb);
            }
        }

        public static void SetVisible(bool b) { if(Instance != null) Instance.Visible = b; }

        //Hide on startup
        static TaskbarIconManager() {
            SetVisible(false);
            Application.ApplicationExit += (sender, args) => { Dispose(); };
        }

        //Delete icon from taskbar
        public static void Dispose() {
            SetVisible(false);
            Instance.Dispose();
        }

    }

}
