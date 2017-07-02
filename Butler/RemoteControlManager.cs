using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Butler.Annotations;
using ModuleAPI;

namespace Butler {

    public class RemoteControlManager: INotifyPropertyChanged {

        private RemoteControlManager() {
            _listener = new TcpListener(IPAddress.Any, Port);
            Command.PseudoCommand += InvokeOnCommandRecieved;
        }
        private static RemoteControlManager _instance;
        public static RemoteControlManager Instance => (_instance = _instance ?? new RemoteControlManager());

        private bool _serverRunning = false;
        public bool ServerRunning {
            get { return _serverRunning; }
            private set { _serverRunning = value; OnPropertyChanged(); OnPropertyChanged(nameof(ServerRunningStatus)); OnPropertyChanged(nameof(ServerStatusColor)); }
        }

        public string ServerRunningStatus => $"{(ServerRunning ? "Stop" : "Start")}";
        public SolidColorBrush ServerStatusColor => ServerRunning ? Brushes.SeaGreen : Brushes.Maroon; 

        private int _port = 4144;
        public int Port {
            get { return _port; }
            set { _port = _port > 65535 || _port < 1024 ? 4144 : value; OnPropertyChanged(); OnPropertyChanged(nameof(PortText)); }
        }
        public string PortText => Port.ToString();

        // Server stuff
        private TcpListener _listener;
        private TcpClient _client;

        public delegate void OnCommandRecieved(string command, TcpClient client);
        public event OnCommandRecieved CommandRecieved;

        public delegate void OnClientConnected(TcpClient client);
        public event OnClientConnected ClientConnected;

        public delegate void OnClientDisconnected(TcpClient client);
        public event OnClientDisconnected ClientDisconnected;
        
        public void StartServer() {
            if(ServerRunning) { return; }

            ServerRunning = true;
            Task.Factory.StartNew(CoreLoop);
        }
        public void StopServer() {
            ServerRunning = false;
        }
        async Task CoreLoop() {
            _listener.Start();

            while (ServerRunning) {
                _client = await _listener.AcceptTcpClientAsync();
                await Task.Factory.StartNew(() => HandleClient(_client));
            }
        }
        
        void HandleClient(TcpClient client) {
            var tcpClient = client;
            var stream = tcpClient.GetStream();
            
            ClientConnected?.Invoke(client);

            var message = new byte[4096];

            while (true) {
                var bytesRead = 0;

                try {
                    bytesRead = stream.Read(message, 0, 4096);
                } catch { /* Clause not empty now, is it ReSharper? Haha! */ }

                // Client disconnected
                if (bytesRead == 0 || !ServerRunning) {
                    ClientDisconnected?.Invoke(client);
                    break;
                }

                var fullMessage = new ASCIIEncoding().GetString(message, 0, bytesRead).Replace("\r", "").Replace("\n", "");
                InvokeOnCommandRecieved(fullMessage, tcpClient);
            }
        }

        // Invoke safely
        void InvokeOnCommandRecieved(string command, TcpClient requester) => CommandRecieved?.Invoke(command, requester);
        /// <summary>
        /// Create a false command from a remote client
        /// </summary>
        /// <param name="command">User input</param>
        /// <param name="requester">Remote client</param>
        public void InvokeOnPseudoCommandRecieved(string command, TcpClient requester) => InvokeOnCommandRecieved(command, requester);

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
