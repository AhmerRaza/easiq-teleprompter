using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Teleprompter.Classes {

    public class CommsClient {
        private Thread tcpThread;      // Receiver
        private bool _conn = false;    // Is connected/connecting?
        private bool _logged = false;  // Is logged in?
        private string _user = "eq_client";          // Username
        private string _pass = "metapass";          // Password
        private bool reg;              // Register mode
        private TcpClient client;
        private NetworkStream netStream;
        private BinaryReader br;
        private BinaryWriter bw;

        public string Server { get { return "localhost"; } }  // Address of server. In this case - local IP address.

        public int Port { get { return 2000; } }

        public bool IsLoggedIn { get { return _logged; } }

        public string UserName { get { return _user; } }

        public string Password { get { return _pass; } }

        // Start connection thread and login or register.
        private void connect(string user, string password, bool register) {
            if (!_conn) {
                _conn = true;
                _user = user;
                _pass = password;
                reg = register;
                tcpThread = new Thread(new ThreadStart(SetupConn));
                tcpThread.Start();
            }
        }

        public void Login() {
            connect(_user, _pass, false);
        }

        public void Register(string user, string password) {
            connect(user, password, true);
        }

        public void Disconnect() {
            if (_conn)
                CloseConn();
        }

        public void IsAvailable(string user) {
            if (_conn) {
                bw.Write(IM_IsAvailable);
                bw.Write(user);
                bw.Flush();
            }
        }

        public void SendMessage(string to, string msg) {
            if (_conn) {
                bw.Write(IM_Send);
                bw.Write(to);
                bw.Write(msg);
                bw.Flush();
            }
        }

        // Events
        public event EventHandler LoginOK;

        public event EventHandler RegisterOK;

        public event IMErrorEventHandler LoginFailed;

        public event IMErrorEventHandler RegisterFailed;

        public event EventHandler Disconnected;

        public event IMAvailEventHandler UserAvailable;

        public event IMReceivedEventHandler MessageReceived;

        virtual protected void OnLoginOK() {
            if (LoginOK != null)
                LoginOK(this, EventArgs.Empty);
        }

        virtual protected void OnRegisterOK() {
            if (RegisterOK != null)
                RegisterOK(this, EventArgs.Empty);
        }

        virtual protected void OnLoginFailed(IMErrorEventArgs e) {
            if (LoginFailed != null)
                LoginFailed(this, e);
        }

        virtual protected void OnRegisterFailed(IMErrorEventArgs e) {
            if (RegisterFailed != null)
                RegisterFailed(this, e);
        }

        virtual protected void OnDisconnected() {
            if (Disconnected != null)
                Disconnected(this, EventArgs.Empty);
        }

        virtual protected void OnUserAvail(IMAvailEventArgs e) {
            if (UserAvailable != null)
                UserAvailable(this, e);
        }

        virtual protected void OnMessageReceived(IMReceivedEventArgs e) {
            if (MessageReceived != null)
                MessageReceived(this, e);
        }

        private void SetupConn()  // Setup connection and login
        {
            client = new TcpClient(Server, Port);  // Connect to the server.
            netStream = client.GetStream();

            br = new BinaryReader(netStream, Encoding.UTF8);
            bw = new BinaryWriter(netStream, Encoding.UTF8);

            // Receive "hello"
            int hello = br.ReadInt32();
            if (hello == IM_Hello) {
                // Hello OK, so answer.
                bw.Write(IM_Hello);
                bw.Write(UserName);
                bw.Write(Password);
                bw.Flush();

                byte ans = br.ReadByte();  // Read answer.
                if (ans == IM_OK) { Receiver(); }
            }
            if (_conn)
                CloseConn();
        }

        private void CloseConn() // Close connection.
        {
            br.Close();
            bw.Close();
            netStream.Close();
            client.Close();
            OnDisconnected();
            _conn = false;
        }

        private void Receiver()  // Receive all incoming packets.
        {
            _logged = true;

            try {
                while (client.Connected)  // While we are connected.
                {
                    byte type = br.ReadByte();  // Get incoming packet type.
                    string msg = br.ReadString();
                    OnMessageReceived(new IMReceivedEventArgs(String.Empty, msg));
                }
            } catch (IOException) { }

            _logged = false;
        }

        // Packet types
        public const int IM_Hello = 2012;      // Hello

        public const byte IM_OK = 0;           // OK
        public const byte IM_Login = 1;        // Login
        public const byte IM_Register = 2;     // Register
        public const byte IM_TooUsername = 3;  // Too long username
        public const byte IM_TooPassword = 4;  // Too long password
        public const byte IM_Exists = 5;       // Already exists
        public const byte IM_NoExists = 6;     // Doesn't exist
        public const byte IM_WrongPass = 7;    // Wrong password
        public const byte IM_IsAvailable = 8;  // Is user available?
        public const byte IM_Send = 9;         // Send message
        public const byte IM_Received = 10;    // Message received
    }
}