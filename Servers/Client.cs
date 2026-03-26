using MultiplayerGameServer.DAO;
using MultiplayerGameServer.Tool;
using SocketGameProtocal;
using System.Net.Sockets;

namespace MultiplayerGameServer.Servers
{
    internal class Client
    {
        private Socket socket;
        private Message message;
        private UserDatabase userDatabase;

        public UserDatabase GetUserDatabase
        {
            get { return userDatabase; }
        }

        public Client(Socket _socket)
        {
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

                message.ReadBuffer(_len);
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
    }
}
