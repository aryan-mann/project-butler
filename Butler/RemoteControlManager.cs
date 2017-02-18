using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Butler {

    public static class RemoteControlManager {

        public static bool ServerRunning { get; private set; }
        public static string ServerRunningStatus => ServerRunning ? "On" : "Off";

        private static int _port = 4144;
        public static int Port {
            get { return _port; }
            set { _port = _port > 65535 || _port < 1024 ? 4144 : value; }
        }
        public static string PortText => Port.ToString();

        // Server stuff
        private static TcpListener _listener;
        private static TcpClient _client;

        public delegate void OnCommandRecieved(string command, TcpClient client);
        public static event OnCommandRecieved CommandRecieved;  

        static RemoteControlManager() {
            try {
                _listener = new TcpListener(IPAddress.Any, 4144);
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }

        public static void StartServer() {
            if(ServerRunning) { return; }

            ServerRunning = true;
            Task.Factory.StartNew(CoreLoop);
        }

        public static void StopServer() {
            ServerRunning = false;
        }
        
        static void CoreLoop() {
            _listener.Start();

            while (ServerRunning) {
                _client = _listener.AcceptTcpClient();

                Task.Factory.StartNew(() => HandleClient(_client));
            }
        }
        
        static void HandleClient(TcpClient client) {
            TcpClient tcpClient = client;
            NetworkStream stream = tcpClient.GetStream();

            System.Diagnostics.Debug.WriteLine("A new client has connected with IP: " + tcpClient.Client.RemoteEndPoint);

            byte[] message = new byte[4096];

            while (true) {
                int bytesRead = 0;

                try {
                    bytesRead = stream.Read(message, 0, 4096);
                } catch { /* Clause not empty now, is it ReSharper? Haha! */ }

                // Client disconnected
                if (bytesRead == 0 || !ServerRunning) {
                    break;
                }

                string fullMessage = new ASCIIEncoding().GetString(message, 0, bytesRead).Replace("\r", "");
                InvokeOnCommandRecieved(fullMessage, tcpClient);
            }
        }

        // Invoke safely
        static void InvokeOnCommandRecieved(string command, TcpClient requester) => CommandRecieved?.Invoke(command, requester);
    }

}
