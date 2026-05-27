using MultiplayerGameServer.Logic;
using MultiplayerGameServer.DAO;
using SocketGameProtocal;
using System.Net;
using System.Net.Sockets;

namespace MultiplayerGameServer.Network
{
    internal class Server
    {
        private Socket socket;
        private List<Client> clientList = new List<Client>();
        private ControllerManager controllerManager;
        public UserDatabase userDatabase;
        private DatabaseConnectionFactory databaseConnectionFactory;

        public Server(int _port)
        {
            databaseConnectionFactory = new DatabaseConnectionFactory();
            userDatabase = new UserDatabase(databaseConnectionFactory);
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

        public void HandleRequest(MainPack _pack, Client _client)
        {
            controllerManager.HandleRequest(_pack, _client);
        }

        public void RemoveClient(Client _client)
        {
            clientList.Remove(_client);
        }
    }
}
