
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;


namespace Butler.Core {
    
    public class HotkeyHandler : IDisposable {

        // 'Hotkey was pressed' message by Windows
        public const int WmHotkey = 0x0312;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private readonly WindowInteropHelper _host;

        public delegate void OnHotkeyPressed();
        public event OnHotkeyPressed HotkeyPressed;

        public HotkeyHandler(Window mainWindow) {
            _host = new WindowInteropHelper(mainWindow);

            SetupHotKey(_host.Handle);
            ComponentDispatcher.ThreadPreprocessMessage += ComponentDispatcher_ThreadPreprocessMessage;
        }

        void ComponentDispatcher_ThreadPreprocessMessage(ref MSG msg, ref bool handled) {
            if (msg.message == WmHotkey) {
                HotkeyPressed?.Invoke();
            }
        }

        private bool SetupHotKey(IntPtr handle) {
            return RegisterHotKey(handle, GetType().GetHashCode(), 0x0008, 0x1B);
        }

        public void Dispose() {
            UnregisterHotKey(_host.Handle, GetType().GetHashCode());
        }
    }

}