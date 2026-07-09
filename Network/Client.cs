using MultiplayerGameServer.Logic.Service;
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
        public bool IsClosing { get; private set; }
        public int UserId { get; set; }
        public long PlayerId { get; set; }

        private Room? currentRoom;
        public Room? CurrentRoom
        {
            get
            {
                if (currentRoom is not null)
                {
                    return currentRoom;
                }

                return null;
            }
            set
            {
                if (value is not null)
                {
                    currentRoom = value;
                    currentRoom.AddClient(this);
                }
                else
                {
                    currentRoom?.RemoveClient(this);
                    currentRoom = null;
                }
            }
        }

        public Client(Socket socket, Server server)
        {
            this.server = server;
            this.socket = socket;
            message = new Message();
            IsClosing = false;

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

        public void Send(MainPack pack) => socket.Send(Message.PackData(pack));

        public void HandleRequest(MainPack pack) => server.HandleRequest(pack, this);

        private void Close()
        {
            if (currentRoom is not null)
            {
                MainPack pack = new MainPack();
                if (currentRoom.isGameRunning)
                {
                    pack.RequestCode = RequestCode.Game;
                    pack.ActionCode = ActionCode.LeaveGame;
                    HandleRequest(pack);
                }

                pack.RequestCode = RequestCode.Room;
                pack.ActionCode = ActionCode.LeaveRoom;
                HandleRequest(pack);
                IsClosing = true;
            }

            server.RemoveClient(this);
            if (socket is not null && socket.Connected)
            {
                socket.Close();
            }
        }
    }
}
