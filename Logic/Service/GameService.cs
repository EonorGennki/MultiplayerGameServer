namespace MultiplayerGameServer.Logic.Service
{
    internal class GameService
    {
        List<Room> roomList;

        public event Action<PlayerInfo>? OnScoreUpdate;

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

        /// <summary>
        /// 更新生命值
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="room"></param>
        /// <param name="deltaHealth">生命值增量</param>
        /// <returns></returns>
        public ServiceResult UpdateHealth(long playerId, Room room, int deltaHealth, long attackPlayerId)
        {
            PlayerInfo? player = room.PlayerList.FirstOrDefault(player => player.PlayerId == playerId);

            if (player is null)
            {
                return ServiceResult.Failure(ServiceErrorCode.UnknownError);
            }

            player.Health += deltaHealth;

            if (player.Health > player.MaxHealth)
            {
                player.Health = player.MaxHealth;
            }

            if (player.Health <= 0)
            {
                player.Health = 0;
                PlayerInfo? attackPlayer = room.PlayerList.FirstOrDefault(player => player.PlayerId == attackPlayerId);
                GainScore(attackPlayer);
            }

            ServiceResult result = ServiceResult.Success();
            result.Data = player.Health;
            return result;
        }

        /// <summary>
        /// 获得分数
        /// </summary>
        /// <param name="attackPlayer"></param>
        private void GainScore(PlayerInfo? attackPlayer)
        {
            if (attackPlayer is null)
            {
                return;
            }

            attackPlayer.Score++;
            OnScoreUpdate?.Invoke(attackPlayer);
        }
    }
}
