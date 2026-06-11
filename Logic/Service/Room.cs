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

        /// <summary>
        /// 创建房间
        /// </summary>
        /// <param name="client"></param>
        /// <param name="roomName"></param>
        /// <param name="maxNum"></param>
        /// <param name="state"></param>
        public Room(RoomInfo roomInfo, PlayerInfo player)
        {
            this.RoomInfo = roomInfo;
            AddPlayer(player);
            SetRoomInfo();
        }

        private void SetRoomInfo() => RoomInfo.currentNum = playerList.Count();

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
