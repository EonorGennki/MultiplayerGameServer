using MultiplayerGameServer.Network;

namespace MultiplayerGameServer.Service
{
    internal class Room
    {
        public string RoomName { get; }
        public int MaxNum { get; }
        public string State { get; }

        private List<Client> clientList = new List<Client>(); //房间内所有客户端

        public Room(Client client, string roomName, int maxNum, string state)
        {
            this.RoomName = roomName;
            this.MaxNum = maxNum;
            this.State = state;
            clientList.Add(client);
        }

        public Room(string roomName, int maxNum, string state)
        {
            this.RoomName = roomName;
            this.MaxNum = maxNum;
            this.State = state;
        }
    }
}
