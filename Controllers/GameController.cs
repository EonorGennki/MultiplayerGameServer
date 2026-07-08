using MultiplayerGameServer.Logic.Service;
using MultiplayerGameServer.Network;
using SocketGameProtocal;

namespace MultiplayerGameServer.Controllers
{
    internal class GameController : BaseController
    {
        private GameService gameService;

        public GameController(GameService gameService)
        {
            requestCode = RequestCode.Game;
            this.gameService = gameService;
        }

        /// <summary>
        /// 离开游戏
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        /// <returns></returns>
        public MainPack LeaveGame(Server server, Client client, MainPack pack)
        {
            if (client.CurrentRoom is null)
            {
                pack.ReturnCode = ReturnCode.Failure;
                pack.ErrorCode = ErrorCode.RoomNotFound;
                return pack;
            }

            ServiceResult result = gameService.LeaveGame(client.CurrentRoom, client.PlayerId);

            if (result.Data is null)
            {
                //房主退出
                client.CurrentRoom.Broadcast(client, pack, server);
            }
            else
            {
                //成员退出
                BroadcastPlayerList(server, client, result);
            }

            return pack;
        }

        public MainPack? UpdateCharacterState(Server server, Client client, MainPack pack)
        {
            if (client.CurrentRoom is null)
            {
                return null;
            }

            client.CurrentRoom.Broadcast(client, pack, server);
            return null;
        }

        /// <summary>
        /// 广播玩家列表
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <param name="result"></param>
        private static void BroadcastPlayerList(Server server, Client client, ServiceResult result)
        {
            if (client.CurrentRoom is null)
            {
                return;   
            }

            List<PlayerInfo> playerList = result.GetData<List<PlayerInfo>>()!;

            MainPack newPack = new MainPack();

            foreach (var player in playerList)
            {
                AddPlayerPack(newPack, player);
            }

            newPack.ActionCode = ActionCode.UpdateCharacterList;
            client.CurrentRoom.Broadcast(client, newPack, server);
        }

        /// <summary>
        /// 添加玩家包
        /// </summary>
        /// <param name="pack"></param>
        /// <param name="player"></param>
        private static void AddPlayerPack(MainPack pack, PlayerInfo player)
        {
            PlayerPack playerPack = new PlayerPack();
            playerPack.PlayerId = player.PlayerId;
            playerPack.PlayerName = player.PlayerName;
            playerPack.Health = player.Health;
            pack.PlayerPack.Add(playerPack);
        }
    }
}
