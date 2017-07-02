
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;


namespace Butler {

    // Oh god, this is some weird PInvoke stuff
    // used to register a global hotkey
    // Sometimes you have to write stuff that
    // is messy and garbage, I am sorry jesus.
    public class HotkeyHandler : IDisposable {

        //Keycode of Windows Key
        public const int WmHotkey = 0x0312;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private WindowInteropHelper _host;

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

        private void SetupHotKey(IntPtr handle) {
            var b = RegisterHotKey(handle, GetType().GetHashCode(), 0x0008, 0x1B);

            Console.WriteLine("Hotkey Registration " + (b ? "Succeeded :)" : "Failed :("));
        }

        public void Dispose() {
            UnregisterHotKey(_host.Handle, GetType().GetHashCode());
        }
    }

}