using MultiplayerGameServer.Tool;
using System.Net.Sockets;

namespace MultiplayerGameServer.Servers
{
    internal class Client
    {
        private Socket socket;
        private Message message;

        public Client(Socket socket)
        {
            this.socket = socket;
            message = new Message();
        }

        void StartReceive()
        {
            socket.BeginReceive(message.Buffer, message.StartIndex, message.RemSize, SocketFlags.None, ReceiveCallback, null);
        }

        void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                if (socket == null || socket.Connected == false)
                {
                    return;
                }
                int len = socket.EndReceive(result);

                if (len == 0)
                {
                    return;
                }

                message.ReadBuffer(len);
                StartReceive();
            }
            catch
            {

            }
        }
    }
}
