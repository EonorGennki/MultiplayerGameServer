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
        public MainPack CreateRoom(Server server, Client client, MainPack pack)
        {
            RoomInfo roomInfo = ParseCreateRoomRequest(pack);

            ServiceResult result = roomService.CreateRoom(client, roomInfo);

            return BuildCreateRoomResponse(client, pack, result);
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
                List<Room> roomList = roomService.SearchRoom().GetData<List<Room>>()!;

                if (roomList.Count <= 0)
                {
                    pack.ReturnCode = ReturnCode.Failure;
                    pack.ErrorCode = ErrorCode.NotFound;
                    return pack;
                }

                foreach (var room in roomList)
                {
                    AddRoomPack(pack, room);
                }
                pack.ReturnCode = ReturnCode.Success;
            }
            catch
            {
                pack.ReturnCode = ReturnCode.Failure;
            }

            return pack;
        }

        /// <summary>
        /// 加入房间
        /// </summary>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        /// <returns></returns>
        public MainPack JoinRoom(Server server, Client client, MainPack pack)
        {
            ServiceResult result = roomService.JoinRoom(client, pack.RoomPack[0].RoomName);

            return BuildJoinRoomResponse(server, client, pack, result);
        }

        /// <summary>
        /// 离开房间
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        /// <returns></returns>
        public MainPack LeaveRoom(Server server, Client client, MainPack pack)
        {
            if (client.CurrentRoom is null)
            {
                pack.ReturnCode = ReturnCode.Failure;
                pack.ErrorCode = ErrorCode.UnknownError;
                return pack;
            }

            ServiceResult result = roomService.LeaveRoom(client);

            return BuildLeaveRoomResponse(server, client, pack, result);
        }

        /// <summary>
        /// 创建房间请求解包
        /// </summary>
        /// <param name="pack"></param>
        /// <returns></returns>
        private RoomInfo ParseCreateRoomRequest(MainPack pack)
        {
            PlayerPack playerList = new PlayerPack();
            RoomPack host = pack.RoomPack[0];
            RoomInfo roomInfo = new RoomInfo(host.RoomName, host.MaxNum, (int)host.StateCode);
            return roomInfo;
        }

        /// <summary>
        /// 创建房间结果打包
        /// </summary>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        private MainPack BuildCreateRoomResponse(Client client, MainPack pack, ServiceResult result)
        {
            if (result.IsSuccess)
            {
                Room room = result.GetData<Room>()!;
                client.CurrentRoom = room;
                PlayerInfo player = room.PlayerList[0];
                pack.RoomPack.Clear();
                AddRoomPack(pack, room);
                AddPlayerPack(player, pack);
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
        /// 加入房间结果打包
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private MainPack BuildJoinRoomResponse(Server server, Client client, MainPack pack, ServiceResult result)
        {
            if (result.IsSuccess)
            {
                Room room = result.GetData<Room>()!;
                client.CurrentRoom = room;
                pack.RoomPack.Clear();
                AddRoomPack(pack, room);
                foreach (PlayerInfo p in room.PlayerList)
                {
                    AddPlayerPack(p, pack);
                }
                pack.ReturnCode = ReturnCode.Success;
                Broadcast(server, client, room);
            }
            else
            {
                pack.ReturnCode = ReturnCode.Failure;
                pack.ErrorCode = (ErrorCode)result.ErrorCode;
            }

            return pack;
        }

        /// <summary>
        /// 离开房间结果打包
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private MainPack BuildLeaveRoomResponse(Server server, Client client, MainPack pack, ServiceResult result)
        {
            if (result.IsSuccess)
            {
                Room room = result.GetData<Room>()!;
                pack.ReturnCode = ReturnCode.Success;
                Broadcast(server, client, room);
            }
            else
            {
                pack.ReturnCode = ReturnCode.Failure;
                pack.ErrorCode = (ErrorCode)result.ErrorCode;
            }

            return pack;
        }

        /// <summary>
        /// 打包广播
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <param name="room"></param>
        private void Broadcast(Server server, Client client, Room room)
        {
            MainPack pack = new MainPack();
            pack.ActionCode = ActionCode.ShowPlayers;
            AddRoomPack(pack, room);
            foreach (PlayerInfo player in room.PlayerList)
            {
                AddPlayerPack(player, pack);
            }
            server.Broadcast(client, pack);
        }

        /// <summary>
        /// 添加玩家包
        /// </summary>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        private void AddPlayerPack(PlayerInfo player, MainPack pack)
        {
            PlayerPack playerPack = new PlayerPack { };
            playerPack.PlayerName = player.playerName;
            pack.PlayerPack.Add(playerPack);
        }

        /// <summary>
        /// 添加房间包
        /// </summary>
        /// <param name="pack"></param>
        /// <param name="room"></param>
        private static void AddRoomPack(MainPack pack, Room room)
        {
            RoomPack roomPack = new RoomPack();
            roomPack.RoomName = room.RoomInfo.RoomName;
            roomPack.CurrentNum = room.RoomInfo.CurrentNum;
            roomPack.MaxNum = room.RoomInfo.MaxNum;
            roomPack.StateCode = (StateCode)room.RoomInfo.State;
            pack.RoomPack.Add(roomPack);
        }
    }
}
