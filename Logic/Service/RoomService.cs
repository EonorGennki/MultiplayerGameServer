using MultiplayerGameServer.Logic.Interface;
using MultiplayerGameServer.Network;

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
        public ServiceResult CreateRoom(Client client, RoomInfo roomInfo)
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
                room.AddPlayer(GetPlayerInfo(client.UserId));
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
        public ServiceResult JoinRoom(Client client, string roomName)
        {
            Room? room = roomList.FirstOrDefault(room => room.RoomInfo.RoomName.Equals(roomName));
            if (room is null)
            {
                return ServiceResult.Failure(ServiceErrorCode.NotFound);
            }

            if (room.RoomInfo.State == 1)
            {
                PlayerInfo player = GetPlayerInfo(client.UserId);
                room.AddPlayer(player);
                SetRoomState(room.RoomInfo);
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
        public ServiceResult LeaveRoom(Client client)
        {
            Room? room = client.CurrentRoom;
            if (room is null)
            {
                return ServiceResult.Failure(ServiceErrorCode.NotFound);
            }

            PlayerInfo? player = room.PlayerList.FirstOrDefault(player => player.playerName == GetPlayerInfo(client.UserId).playerName);
            if (player is null)
            {
                return ServiceResult.Failure(ServiceErrorCode.UnknownError);
            }
            
            if (player == room.PlayerList[0])
            {
                room.PlayerList.Clear();
                roomList.Remove(room);
            }

            room.RemovePlayer(player);
            client.CurrentRoom = null;

            ServiceResult result = ServiceResult.Success();
            result.Data = room;
            return result;
        }



        private PlayerInfo GetPlayerInfo(int userId) => userService.GetPlayerInfo(userId);

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
