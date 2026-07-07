using MultiplayerGameServer.Logic.Interface;
using System.Collections.Concurrent;

namespace MultiplayerGameServer.Logic.Service
{
    internal class RoomService
    {
        private readonly IUserService userService;
        private List<Room> roomList;

        public event Action<Room, int>? OnCountDownTick;
        public event Action<Room>? OnGameStart;

        private ConcurrentDictionary<Room, System.Timers.Timer> timers = new ConcurrentDictionary<Room, System.Timers.Timer>();
        //房间独立锁
        private readonly ConcurrentDictionary<Room, object> roomLocks = new ConcurrentDictionary<Room, object>();

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
        public ServiceResult CreateRoom(long playerId, RoomInfo roomInfo)
        {
            bool exists = roomList.Any(room => room.RoomInfo.RoomName.Equals(roomInfo.RoomName));
            if (exists)
            {
                return ServiceResult.Failure(ServiceErrorCode.RoomAlreadyExists);
            }

            try
            {
                Room room = new Room(roomInfo);
                roomList.Add(room);
                PlayerInfo player = GetPlayerInfo(playerId);
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
        /// <param name="playerId"></param>
        /// <param name="roomInfo"></param>
        /// <returns></returns>
        public ServiceResult JoinRoom(long playerId, RoomInfo roomInfo)
        {
            Room? room = roomList.FirstOrDefault(room => room.RoomInfo.RoomName.Equals(roomInfo.RoomName));
            if (room is null)
            {
                return ServiceResult.Failure(ServiceErrorCode.RoomNotFound);
            }

            if (room.RoomInfo.State == 1)
            {
                PlayerInfo player = GetPlayerInfo(playerId);
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
        /// <param name="room"></param>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public ServiceResult LeaveRoom(Room? room, long playerId)
        {
            if (room is null)
            {
                return ServiceResult.Failure(ServiceErrorCode.RoomNotFound);
            }

            PlayerInfo? player = room.PlayerList.FirstOrDefault(player => player.PlayerName == GetPlayerInfo(playerId).PlayerName);
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

        /// <summary>
        /// 合成聊天消息
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public ServiceResult Chat(long playerId, string text)
        {
            PlayerInfo player = GetPlayerInfo(playerId);
            string chatText = player.PlayerName + "：" + text;

            ServiceResult result = ServiceResult.Success();
            result.Data = chatText;
            return result;
        }

        /// <summary>
        /// 准备游戏
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public ServiceResult Ready(long playerId, Room? room)
        {
            if (room is null)
            {
                return ServiceResult.Failure(ServiceErrorCode.RoomNotFound);
            }

            PlayerInfo? player = room.PlayerList.FirstOrDefault(player => player.PlayerId == playerId);

            if (player is null)
            {
                return ServiceResult.Failure(ServiceErrorCode.UserNotFound);
            }

            player.ToggleIsReady(player.IsReady);
            ServiceResult result = ServiceResult.Success();
            result.Data = player;
            return result;
        }

        /// <summary>
        /// 开始游戏倒计时
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        public ServiceResult StartGameCountDown(Room? room)
        {
            if (room is null)
            {
                return ServiceResult.Failure(ServiceErrorCode.RoomNotFound);
            }

            if (room.PlayerList.Count <= 1)
            {
                
            }

            foreach (var player in room.PlayerList)
            {
                if (!player.IsReady)
                {
                    return ServiceResult.Failure(ServiceErrorCode.PlayerNotReady);
                }
            }

            var roomLock = roomLocks.GetOrAdd(room, new object());

            lock (roomLock)
            {
                if (room.isGameRunning)
                {
                    return ServiceResult.Failure(ServiceErrorCode.GameAlreadyStarted);
                }

                StopTimer(room);

                System.Timers.Timer timer = new System.Timers.Timer(1000);
                int seconds = 5;
                OnCountDownTick?.Invoke(room, seconds);

                timer.Elapsed += (sender, e) =>
                {
                    lock (roomLock)
                    {
                        seconds--;

                        if (seconds > 0)
                        {
                            OnCountDownTick?.Invoke(room, seconds);
                        }
                        else
                        {
                            timer.Stop();
                            timer.Dispose();
                            timers.TryRemove(room, out _);

                            StartGame(room);
                        }

                    }
                };

                timer.Start();
                timers.AddOrUpdate(room, timer, (room, timer) => timer);
            }
            return ServiceResult.Success();
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        /// <param name="room"></param>
        private void StartGame(Room room)
        {
            room.isGameRunning = true;

            OnGameStart?.Invoke(room);
        }

        /// <summary>
        /// 销毁旧计时器
        /// </summary>
        /// <param name="room"></param>
        private void StopTimer(Room room)
        {
            if (timers.TryRemove(room, out System.Timers.Timer? timer))
            {
                timer.Stop();
                timer.Dispose();
            }
        }

        private PlayerInfo GetPlayerInfo(long playerId) => userService.GetPlayerInfo(playerId);
    }
}
