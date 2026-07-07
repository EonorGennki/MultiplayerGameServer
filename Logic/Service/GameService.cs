namespace MultiplayerGameServer.Logic.Service
{
    internal class GameService
    {
        List<Room> roomList;

        public GameService(List<Room> roomList)
        {
            this.roomList = roomList;
        }

        /// <summary>
        /// 成员离开游戏
        /// </summary>
        /// <param name="room"></param>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public ServiceResult LeaveGame(Room room, long playerId)
        {
            //房主退出
            if (playerId == room.PlayerList[0].PlayerId)
            {
                roomList.Remove(room);
                return ServiceResult.Success();
            }

            List<PlayerInfo> playerList = new List<PlayerInfo>(room.PlayerList);

            //将发送请求的PlayerId放在列表第一位
            PlayerInfo? target = playerList.Find(p => p.PlayerId == playerId);

            if (target is not null)
            {
                room.PlayerList.Remove(target);
                playerList.Remove(target);
                playerList.Insert(0, target);
            }

            ServiceResult result = ServiceResult.Success();
            result.Data = playerList;
            return result;
        }
    }
}
