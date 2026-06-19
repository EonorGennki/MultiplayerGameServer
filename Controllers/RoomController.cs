using MultiplayerGameServer.Logic.Service;
using MultiplayerGameServer.Network;
using SocketGameProtocal;

namespace MultiplayerGameServer.Controllers
{
    internal class RoomController : BaseController
    {
        private RoomService roomService;

        private Server? server;

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
            RoomInfo roomInfo = ExtractRoomInfo(pack);

            ServiceResult result = roomService.CreateRoom(client.UserId, roomInfo);

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
                    pack.ErrorCode = ErrorCode.RoomNotFound;
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
            RoomInfo roomInfo = ExtractRoomInfo(pack);

            ServiceResult result = roomService.JoinRoom(client.UserId, roomInfo);

            return BuildJoinRoomResponse(server, client, pack, result);
        }

        /// <summary>
        /// 离开房间
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        /// <returns></returns>
        public MainPack? LeaveRoom(Server server, Client client, MainPack pack)
        {
            ServiceResult result = roomService.LeaveRoom(client.CurrentRoom, client.UserId);

            if (result.IsSuccess)
            {
                client.CurrentRoom = null;
            }

            //连接断开处理
            if (client.isClosing == true)
            {
                Room room = result.GetData<Room>()!;
                UpdatePlayerList(server, client, room);
                return null;
            }

            return BuildLeaveRoomResponse(server, client, pack, result);
        }

        /// <summary>
        /// 聊天
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        /// <returns></returns>
        public MainPack Chat(Server server, Client client, MainPack pack)
        {
            string text = pack.Text;
            ServiceResult result = roomService.Chat(client.UserId, text);
            pack.ReturnCode = ReturnCode.Success;
            pack.Text = result.GetData<string>();

            server.Broadcast(client, pack);
            return pack;
        }

        /// <summary>
        /// 准备游戏
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        /// <returns></returns>
        public MainPack Ready(Server server, Client client, MainPack pack)
        {
            ServiceResult result = roomService.Ready(client.UserId, client.CurrentRoom);
            PlayerInfo player = result.GetData<PlayerInfo>()!;
            AddPlayerPack(player, pack);
            server.Broadcast(client, pack);

            return pack;
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        /// <returns></returns>
        public MainPack StartGame(Server server, Client client, MainPack pack)
        {
            this.server = server;

            roomService.OnCountDownTick += OnCountDownTick;
            roomService.OnGameStart += OnGameStart;
            ServiceResult result = roomService.StartGameCountDown(client.CurrentRoom);

            if (result.IsSuccess)
            {
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
        /// 提取房间信息
        /// </summary>
        /// <param name="pack"></param>
        /// <returns></returns>
        private RoomInfo ExtractRoomInfo(MainPack pack)
        {
            PlayerPack playerList = new PlayerPack();
            RoomPack room = pack.RoomPack[0];
            RoomInfo roomInfo = new RoomInfo(room.RoomName, room.MaxNum, (int)room.StateCode);
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
                UpdatePlayerList(server, client, room);  //通知其他客户端刷新
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
                UpdatePlayerList(server, client, room);
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
        /// 更新其他客户端玩家列表
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <param name="room"></param>
        private void UpdatePlayerList(Server server, Client client, Room room)
        {
            MainPack pack = new MainPack();

            if (room.PlayerList.Count <= 0)
            {
                pack.ActionCode = ActionCode.LeaveRoom;
                server.Broadcast(client, pack);
                return;
            }

            AddRoomPack(pack, room);
            pack.ActionCode = ActionCode.ShowPlayers;
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
            PlayerPack playerPack = new PlayerPack();
            playerPack.PlayerId = player.playerId;
            playerPack.PlayerName = player.playerName;
            playerPack.IsReady = player.isReady;
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

        private void OnCountDownTick(Room room, int seconds)
        {
            MainPack pack = new MainPack();
            pack.ActionCode = ActionCode.Chat;
            pack.Text = seconds.ToString() + "...";

            server?.Broadcast(null, pack); //不可能为空
        }

        private void OnGameStart(Room room)
        {
            try
            {
                MainPack pack = new MainPack();
                pack.ActionCode = ActionCode.Chat;
                pack.Text = "游戏开始";

                server?.Broadcast(null, pack); //不可能为空

                pack.ActionCode = ActionCode.CanStart;
                server?.Broadcast(null, pack);
            }
            finally
            {
                server = null;
                roomService.OnCountDownTick += OnCountDownTick;
                roomService.OnGameStart += OnGameStart;
            }
        }
    }
}
