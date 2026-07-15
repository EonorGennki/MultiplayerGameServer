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
                room.isGameRunning = false;
                return ServiceResult.Success();
            }

            List<PlayerInfo> playerList = new List<PlayerInfo>(room.PlayerList);

            //将发送请求的PlayerId放在列表第一位
            PlayerInfo? target = playerList.Find(p => p.PlayerId == playerId);

            if (target is not null)
            {
                room.RemovePlayer(target);
                playerList.Remove(target);
                playerList.Insert(0, target);
            }

            ServiceResult result = ServiceResult.Success();
            result.Data = playerList;
            return result;
        }


        public ServiceResult CalculateHealth(long playerId, int damage, Room room)
        {
            PlayerInfo? player = room.PlayerList.FirstOrDefault(player => player.PlayerId == playerId);

            if (player is null)
            {
                return ServiceResult.Failure(ServiceErrorCode.UnknownError);
            }

            player.Health -= damage;
            ServiceResult result = ServiceResult.Success();
            result.Data = player.Health;
            return result;
        }
    }
}
