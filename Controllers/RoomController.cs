using MultiplayerGameServer.Logic.Service;
using MultiplayerGameServer.Network;
using SocketGameProtocal;

namespace MultiplayerGameServer.Controllers
{
    internal class RoomController : BaseController
    {
        private RoomService roomService;

        public RoomController(RoomService roomService)
        {
            requestCode = RequestCode.Room;
            this.roomService = roomService;
        }

        /// <summary>
        /// 创建房间
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        /// <returns></returns>
        public MainPack CreateRoom(Client client, MainPack pack)
        {
            RoomInfo roomInfo = ParseCreateRoomRequest(pack);

            ServiceResult result = roomService.CreateRoom(client, roomInfo);

            return BuildCreateRoomResponse(client, pack, result);
        }

        /// <summary>
        /// 打包
        /// </summary>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        private MainPack BuildCreateRoomResponse(Client client, MainPack pack, ServiceResult result)
        {
            if (result.IsSuccess)
            {
                pack.PlayerPack[0].PlayerName = roomService.GetUsername(client);
                pack.RoomPack[0].StateCode = StateCode.Waiting;
                pack.ReturnCode = ReturnCode.Success;
            }
            else
            {
                pack.ReturnCode = ReturnCode.Failure;
                pack.ErrorCode = (ErrorCode)result.ErrorCode;
            }
            return pack;
        }

        /// <summary>
        /// 解包
        /// </summary>
        /// <param name="pack"></param>
        /// <returns></returns>
        private static RoomInfo ParseCreateRoomRequest(MainPack pack)
        {
            PlayerPack playerList = new PlayerPack();
            RoomPack host = pack.RoomPack[0];
            RoomInfo roomInfo = new RoomInfo(host.RoomName, host.MaxNum, host.StateCode.ToString());
            return roomInfo;
        }

        /// <summary>
        /// 搜索房间
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        /// <returns></returns>
        public MainPack SearchRoom(Client client, MainPack pack)
        {
            try
            {
                List<Room> roomList = roomService.SearchRoom().GetData<List<Room>>()!;
                pack.RoomPack.Clear();

                if (roomList.Count <= 0)
                {
                    pack.ReturnCode = ReturnCode.Failure;
                    pack.ErrorCode = ErrorCode.NotFound;
                    return pack;
                }

                foreach (var room in roomList)
                {
                    RoomPack roomPack = new RoomPack();
                    roomPack.RoomName = room.roomInfo.RoomName;
                    roomPack.MaxNum = room.roomInfo.MaxNum;
                    if (Enum.TryParse<StateCode>(room.roomInfo.State, out var statecode))
                    {
                        roomPack.StateCode = statecode;
                    }
                    else
                    {
                        roomPack.StateCode = StateCode.StateNone;
                    }
                        roomPack.StateCode = (StateCode)Enum.Parse(typeof(StateCode), room.roomInfo.State);
                    pack.RoomPack.Add(roomPack);
                }
                pack.ReturnCode = ReturnCode.Success;
            }
            catch
            {
                pack.ReturnCode = ReturnCode.Failure;
            }

            return pack;
        }

        public MainPack JoinRoom(Client client, MainPack pack)
        {
            bool success = roomService.JoinRoom(pack.RoomPack[0].RoomName);

            return pack;
        }

        private ErrorCode GetErrorCode(int errorCode)
        {
            switch (errorCode)
            {
                case 2001:
                    return ErrorCode.AlreadyExits;
                case 2002:
                    return ErrorCode.NotFound;
                default:
                    return ErrorCode.ErrNone;
            }
        }
    }
}
