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

        public Server(int port)
        {
            databaseConnectionFactory = new DatabaseConnectionFactory();
            userDatabase = new UserDatabase(databaseConnectionFactory);
            controllerManager = new ControllerManager(this);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Any, port));
            socket.Listen(0);
            StartAccept();
        }

        void StartAccept()
        {
            socket.BeginAccept(AcceptCallback, null);
        }

        void AcceptCallback(IAsyncResult result)
        {
            Socket client = socket.EndAccept(result);
            clientList.Add(new Client(client, this));
            StartAccept();
        }

        public void HandleRequest(MainPack pack, Client client)
        {
            controllerManager.HandleRequest(pack, client);
        }

        public void RemoveClient(Client client)
        {
            clientList.Remove(client);
        }
    }
}
