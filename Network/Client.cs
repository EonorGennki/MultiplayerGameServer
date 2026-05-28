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

        public Client(Socket socket, Server server)
        {
            this.server = server;
            this.socket = socket;
            message = new Message();

            StartReceive();
        }

        void StartReceive()
        {
            socket.BeginReceive(message.Buffer, message.StartIndex, message.RemSize, SocketFlags.None, ReceiveCallback, null);
        }

        void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                if (socket is null || !socket.Connected)
                {
                    Close();
                    return;
                }

                int len = socket.EndReceive(result);

                if (len == 0)
                {
                    Close();
                    return;
                }

                message.ReadBuffer(len, HandleRequest);
                StartReceive();
            }
            catch
            {
                Close();
            }
        }

        public void Send(MainPack pack)
        {
            socket.Send(Message.PackData(pack));
        }

        void HandleRequest(MainPack pack)
        {
            server.HandleRequest(pack, this);
        }

        private void Close()
        {
            server.RemoveClient(this);
            if (socket is not null && socket.Connected)
            {
                socket.Close();
            }
        }
    }
}
