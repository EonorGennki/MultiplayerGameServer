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
            bool exists = roomList.Any(room => room.roomInfo.RoomName.Equals(roomInfo.RoomName));

            if (exists)
            {
                return ServiceResult.Failure(ServiceErrorCode.AlreadyExists);
            }

            try
            {
                Room room = new Room(client, roomInfo);
                roomList.Add(room);
                return ServiceResult.Success();
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

        public bool JoinRoom(string roomName)
        {
            bool isFounded = roomList.Any(room => room.roomInfo.RoomName.Equals(roomName));
            if (isFounded)
            {
                
            }
            return false;
        }

        public string GetUsername(Client client) => userService.GetUsername(client.userId);
    }
}
