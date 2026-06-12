namespace MultiplayerGameServer.Logic.Service
{
    internal class Room
    {
        public RoomInfo RoomInfo { get; }

        //房间内所有玩家
        private List<PlayerInfo> playerList = new List<PlayerInfo>();
        public List<PlayerInfo> PlayerList
        {
            get => playerList;
        }

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
    }
}
