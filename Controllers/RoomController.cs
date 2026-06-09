using MultiplayerGameServer.Network;
using MultiplayerGameServer.Service;
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
        public MainPack CreateRoom(Server server, Client client, MainPack pack)
        {
            RoomPack host = pack.RoomPack[0];
            string roomName = host.RoomName;
            int maxNum = host.MaxNum;
            string state = host.StateCode.ToString();
            bool success = roomService.CreateRoom(client, roomName, maxNum, state);
            if (success)
            {
                pack.RoomPack[0].StateCode = StateCode.Waiting;
                pack.ReturnCode = ReturnCode.Success;
            }
            else
            {
                pack.ReturnCode = ReturnCode.Failure;
            }
            return pack;
        }

        /// <summary>
        /// 搜索房间
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        /// <returns></returns>
        public MainPack SearchRoom(Server server, Client client, MainPack pack)
        {
            try
            {
                List<Room> roomList = roomService.SearchRoom();
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
                    roomPack.RoomName = room.RoomName;
                    roomPack.MaxNum = room.MaxNum;
                    roomPack.StateCode = (StateCode)Enum.Parse(typeof(StateCode), room.State);
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
    }
}
