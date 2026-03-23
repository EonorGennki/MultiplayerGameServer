using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace MultiplayerGameServer.Servers
{
    internal class Server
    {
        private Socket socket;
        private List<Client> clientList = new List<Client>();

        Server(int port)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Any, port));
            socket.Listen(0);
        }

        void StartAccept()
        {
            socket.BeginAccept(AcceptCallback, null);
        }

        void AcceptCallback(IAsyncResult result)
        {
            Socket client = socket.EndAccept(result);
            clientList.Add(new Client(client));
            StartAccept();
        }
    }
}
