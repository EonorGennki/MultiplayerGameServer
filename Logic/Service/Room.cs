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
    }
}
