using MultiplayerGameServer.Controllers;
using MultiplayerGameServer.DAO;
using MultiplayerGameServer.Logic.Service;
using SocketGameProtocal;
using System.Net;
using System.Net.Sockets;

namespace MultiplayerGameServer.Network
{
    internal class Server
    {
        private Socket socket;
        private List<Client> clientList = new List<Client>();
        private List<Room> roomList = new List<Room>();
        private ControllerManager controllerManager;
        private ServiceGroup serviceGroup;
        private DatabaseConnectionFactory databaseConnectionFactory;
        private Database database;

        public Server(int port)
        {
            databaseConnectionFactory = new DatabaseConnectionFactory();
            database = new Database(databaseConnectionFactory);
            serviceGroup = new ServiceGroup(database, roomList);
            controllerManager = new ControllerManager(this, serviceGroup);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Any, port));
            socket.Listen(0);
            StartAccept();
        }

        void StartAccept() => socket.BeginAccept(AcceptCallback, null);

        void AcceptCallback(IAsyncResult result)
        {
            Socket client = socket.EndAccept(result);
            clientList.Add(new Client(client, this));
            StartAccept();
        }

        public void HandleRequest(MainPack pack, Client client) => controllerManager.HandleRequest(this, client, pack);

        public void RemoveClient(Client client) => clientList.Remove(client);

        /// <summary>
        /// 广播
        /// </summary>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        public void Broadcast(Client? client, MainPack pack)
        {
            foreach (Client c in clientList)
            {
                if (c.Equals(client))
                {
                    continue;
                }
                c.Send(pack);
            }
        }
    }
}
