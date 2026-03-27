using MultiplayerGameServer.Controller;
using SocketGameProtocal;
using System.Net;
using System.Net.Sockets;

namespace MultiplayerGameServer.Servers
{
    internal class Server
    {
        private Socket socket;
        private List<Client> clientList = new List<Client>();
        private ControllerManager controllerManager;

        Server(int _port)
        {
            controllerManager = new ControllerManager(this);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Any, _port));
            socket.Listen(0);
            StartAccept();
        }

        void StartAccept()
        {
            socket.BeginAccept(AcceptCallback, null);
        }

        void AcceptCallback(IAsyncResult _result)
        {
            Socket _client = socket.EndAccept(_result);
            clientList.Add(new Client(_client, this));
            StartAccept();
        }

        public bool SignUp(Client _client, MainPack _pack)
        {
            return _client.GetUserDatabase.SignUp(_pack);
        }

        public void HandleRequest(MainPack _pack, Client _client)
        {
            controllerManager.HandleRequest(_pack, _client);
        }
    }
}
