using MultiplayerGameServer.Network;
using SocketGameProtocal;
using System.Diagnostics;

namespace MultiplayerGameServer.Logic.Service
{
    internal class Room
    {
        public RoomInfo RoomInfo { get; }

        //房间内所有玩家
        private List<Client> clientList = new List<Client>();
        private List<PlayerInfo> playerList = new List<PlayerInfo>();
        public List<PlayerInfo> PlayerList
        {
            get => playerList;
        }

        public bool isGameRunning;

        public Room(RoomInfo roomInfo)
        {
            this.RoomInfo = roomInfo;
            SetRoomInfo();
        }

        private void SetRoomInfo() => RoomInfo.CurrentNum = playerList.Count();

        public void AddPlayer(PlayerInfo player)
        {
            playerList.Add(player);
            SetRoomInfo();
        }

        public void RemovePlayer(PlayerInfo player)
        {
            playerList.Remove(player);
            SetRoomInfo();
        }

        public void AddClient(Client client)
        {
            clientList.Add(client);
        }

        public void RemoveClient(Client client)
        {
            clientList.Remove(client);
        }

        public void SetRoomState(RoomInfo roomInfo)
        {
            if (roomInfo.CurrentNum < roomInfo.MaxNum)
            {
                roomInfo.State = 1; //Waiting
            }
            else if (roomInfo.CurrentNum >= roomInfo.MaxNum)
            {
                roomInfo.State = 2; //Full
            }
        }

        public void Broadcast(Client? client, MainPack pack, Server server)
        {
            server.Broadcast(client, clientList, pack);
        }
    }
}
