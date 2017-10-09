using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Teleprompter.Classes {

    public class Tcpclient {
        private TcpClient client;
        private String myIP = "";

        public Tcpclient() {
            client = new TcpClient();
            myIP = GetMyIP(); //testing only -- must change to entered ip address
        }

        public String MyIP {
            get { return myIP; }
            set { myIP = value; }
        }

        public bool ConnectToServer(String serverIP, out String message) {
            try {
                IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), 3000);
                client.Connect(serverEndPoint);
                NetworkStream clientStream = client.GetStream();
                ASCIIEncoding encoder = new ASCIIEncoding();
                byte[] buffer = encoder.GetBytes("Hello Server!");
                clientStream.Write(buffer, 0, buffer.Length);
                clientStream.Flush();
                message = "";
                return true;
            } catch (Exception ex) {
                message = ex.Message;
                return false;
            }
        }

        public bool ConnectToServer(String serverIP, String outMessage, out String message) {
            try {
                if (!client.Connected) {
                    IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), 3000);
                    client.Connect(serverEndPoint);
                }
                if (client.Connected) {
                    NetworkStream clientStream = client.GetStream();
                    ASCIIEncoding encoder = new ASCIIEncoding();
                    byte[] buffer = encoder.GetBytes(outMessage);
                    clientStream.Write(buffer, 0, buffer.Length);
                    clientStream.Flush();
                    message = "";
                    return true;
                } else {
                    message = "No client";
                    return true;
                }
            } catch (Exception ex) {
                message = ex.Message;
                return false;
            }
        }

        public String GetMyIP() {
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList) {
                if (ip.AddressFamily == AddressFamily.InterNetwork) {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }
    }
}