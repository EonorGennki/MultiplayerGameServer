using MultiplayerGameServer.Logic.Interface;

namespace MultiplayerGameServer.Logic.Service
{
    internal class RoomService
    {
        private readonly IUserService userService;
        private List<Room> roomList;

        public RoomService(IUserService userService, List<Room> roomList)
        {
            this.userService = userService;
            this.roomList = roomList;
        }

        /// <summary>
        /// 创建房间
        /// </summary>
        /// <param name="client"></param>
        /// <param name="roomInfo"></param>
        /// <returns></returns>
        public ServiceResult CreateRoom(int userId, RoomInfo roomInfo)
        {
            bool exists = roomList.Any(room => room.RoomInfo.RoomName.Equals(roomInfo.RoomName));
            if (exists)
            {
                return ServiceResult.Failure(ServiceErrorCode.AlreadyExists);
            }

            try
            {
                Room room = new Room(roomInfo);
                roomList.Add(room);
                PlayerInfo player = GetPlayerInfo(userId);
                player.SetIsReady(true);
                room.AddPlayer(player);
                ServiceResult result = ServiceResult.Success();
                result.Data = room;
                return result;
            }
            catch
            {
                return ServiceResult.Failure(ServiceErrorCode.UnknownError);
            }
        }

        /// <summary>
        /// 搜索房间
        /// </summary>
        /// <returns></returns>
        public ServiceResult SearchRoom()
        {
            ServiceResult result = ServiceResult.Success();
            result.Data = roomList;
            return result;
        }

        /// <summary>
        /// 加入房间
        /// </summary>
        /// <param name="client"></param>
        /// <param name="roomName"></param>
        /// <returns></returns>
        public ServiceResult JoinRoom(int userId, RoomInfo roomInfo)
        {
            Room? room = roomList.FirstOrDefault(room => room.RoomInfo.RoomName.Equals(roomInfo.RoomName));
            if (room is null)
            {
                return ServiceResult.Failure(ServiceErrorCode.NotFound);
            }

            if (room.RoomInfo.State == 1)
            {
                PlayerInfo player = GetPlayerInfo(userId);
                player.SetIsReady(false);
                room.AddPlayer(player);
                room.SetRoomState(room.RoomInfo);
                ServiceResult result = ServiceResult.Success();
                result.Data = room;
                return result;
            }
            else if (room.RoomInfo.State == 2)
            {
                ServiceResult result = ServiceResult.Failure(ServiceErrorCode.RoomIsFull);
            }
            else if (room.RoomInfo.State == 3)
            {
                ServiceResult result = ServiceResult.Failure(ServiceErrorCode.GameAlreadyStarted);
            }

            return ServiceResult.Failure(ServiceErrorCode.UnknownError);
        }

        /// <summary>
        /// 离开房间
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public ServiceResult LeaveRoom(Room room, int userId)
        {
            if (room is null)
            {
                return ServiceResult.Failure(ServiceErrorCode.NotFound);
            }

            PlayerInfo? player = room.PlayerList.FirstOrDefault(player => player.playerName == GetPlayerInfo(userId).playerName);
            if (player is null)
            {
                return ServiceResult.Failure(ServiceErrorCode.UnknownError);
            }

            if (player == room.PlayerList[0])
            {
                room.PlayerList.Clear();
                roomList.Remove(room);
            }

            player.SetIsReady(false);
            room.RemovePlayer(player);
            room.SetRoomState(room.RoomInfo);
            ServiceResult result = ServiceResult.Success();
            result.Data = room;
            return result;
        }

        public ServiceResult Chat(int userId, string text)
        {
            PlayerInfo player = GetPlayerInfo(userId);
            string chatText = player.playerName + "：" + text;

            ServiceResult result = ServiceResult.Success();
            result.Data = chatText;
            return result;
        }

        public ServiceResult StartGame()
        {
            return ServiceResult.Success();
        }

        public ServiceResult Ready(int userId)
        {
            PlayerInfo player = GetPlayerInfo(userId);
            player.ToggleIsReady(player.isReady);
            ServiceResult result = ServiceResult.Success();
            result.Data = player;
            return result;
        }

        private PlayerInfo GetPlayerInfo(int userId) => userService.GetPlayerInfo(userId);
    }
}
