using MultiplayerGameServer.Tool;
using SocketGameProtocal;
using System.Net.Sockets;

namespace MultiplayerGameServer.Network
{
    internal class Client
    {
        private Socket socket;
        private Server server;
        private Message message;

        public Client(Socket _socket, Server _server)
        {
            server = _server;
            socket = _socket;
            message = new Message();

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
                    Close();
                    return;
                }

                int _len = socket.EndReceive(_result);

                if (_len == 0)
                {
                    Close();
                    return;
                }

                message.ReadBuffer(_len, HandleRequest);
                StartReceive();
            }
            catch
            {
                Close();
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

        private void Close()
        {
            server.RemoveClient(this);
            if (socket is not null)
            {
                socket.Close();
            }
        }
    }
}
