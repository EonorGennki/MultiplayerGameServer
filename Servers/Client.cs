using MultiplayerGameServer.DAO;
using MultiplayerGameServer.Tool;
using Org.BouncyCastle.Tls;
using SocketGameProtocal;
using System.Net.Sockets;

namespace MultiplayerGameServer.Servers
{
    internal class Client
    {
        private Socket socket;
        private Server server;
        private Message message;
        private UserDatabase userDatabase;

        public UserDatabase GetUserDatabase
        {
            get { return userDatabase; }
        }
         
        public Client(Socket _socket, Server _server)
        {
            server = _server;
            socket = _socket;
            message = new Message();
            userDatabase = new UserDatabase();

            StartReceive();
        }

        void StartReceive()
        {
            socket.BeginReceive(message.Buffer, message.StartIndex, message.RemSize, SocketFlags.None, ReceiveCallback, null);
        }

        void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                if (socket is null || !socket.Connected)
                {
                    return;
                }
                int _len = socket.EndReceive(_result);

                if (_len == 0)
                {
                    return;
                }

                message.ReadBuffer(_len, HandleRequest);
                StartReceive();
            }
            catch
            {

            }
        }

        public void Send(MainPack _pack)
        {
            socket.Send(Message.PackData(_pack));
        }

        void HandleRequest(MainPack _pack)
        {
            server.HandleRequest(_pack, this);
        }
    }
}
