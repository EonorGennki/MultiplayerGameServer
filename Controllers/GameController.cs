using MultiplayerGameServer.Logic.Service;
using MultiplayerGameServer.Network;
using SocketGameProtocal;

namespace MultiplayerGameServer.Controllers
{
    internal class GameController : BaseController
    {
        private GameService gameService;
        private Client? client;
        private Server? server;

        public GameController(GameService gameService)
        {
            requestCode = RequestCode.Game;
            this.gameService = gameService;
            gameService.OnScoreUpdate += OnScoreUpdate;
        }

        /// <summary>
        /// 离开游戏
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        /// <returns></returns>
        public MainPack? LeaveGame(Server server, Client client, MainPack pack)
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
                client.CurrentRoom = null;
            }
            else
            {
                //成员退出
                BroadcastPlayerList(server, client, result);
                client.CurrentRoom = null;
            }

            if (client.IsClosing)
            {
                return null;
            }

            return pack;
        }

        /// <summary>
        /// 位置同步
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        /// <returns></returns>
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
        /// 更新生命值
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        /// <returns></returns>
        public MainPack? UpdateHealth(Server server, Client client, MainPack pack)
        {
            this.client = client;
            this.server = server;

            if (client.CurrentRoom is null)
            {
                return null;
            }

            long playerId = pack.PlayerPack[0].PlayerId;
            int dealdaHealth = pack.PlayerPack[0].DeltaHealth;
            long attackPlayerId = pack.PlayerPack[0].AttackPlayerId;

            ServiceResult result = gameService.UpdateHealth(playerId, client.CurrentRoom, dealdaHealth, attackPlayerId);

            BuildUpdateHealthResponse(pack, result);

            client.CurrentRoom.Broadcast(client, pack, server);
            return pack;
        }

        /// <summary>
        /// 游戏结束
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        private MainPack GameOver(Server server, Client client, MainPack pack)
        {
            if (client.CurrentRoom is null)
            {
                pack.ReturnCode = ReturnCode.Failure;
                return pack;
            }

            long playerId = pack.PlayerPack[0].PlayerId;

            ServiceResult result = gameService.WhoisWinner(client.CurrentRoom, playerId);

            pack.PlayerPack[0].IsWinner = result.GetValue<bool>();

            return pack;
        }

        /// <summary>
        /// 生命值更新结果打包
        /// </summary>
        /// <param name="pack"></param>
        /// <param name="result"></param>
        private void BuildUpdateHealthResponse(MainPack pack, ServiceResult result)
        {
            int health = result.GetValue<int>();

            pack.PlayerPack[0].Health = health;

            if (health <= 0)
            {
                pack.PlayerPack[0].IsDead = true;
            }
        }

        /// <summary>
        /// 广播更新玩家列表
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <param name="result"></param>
        private void BroadcastPlayerList(Server server, Client client, ServiceResult result)
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
        private void AddPlayerPack(MainPack pack, PlayerInfo player)
        {
            PlayerPack playerPack = new PlayerPack();
            playerPack.PlayerId = player.PlayerId;
            playerPack.PlayerName = player.PlayerName;
            playerPack.Health = player.Health;
            pack.PlayerPack.Add(playerPack);
        }

        private void OnScoreUpdate(PlayerInfo playerInfo)
        {
            MainPack pack = new MainPack();
            pack.ActionCode = ActionCode.GainScore;

            PlayerPack playerPack = new PlayerPack();
            playerPack.AttackPlayerId = playerInfo.PlayerId;
            playerPack.Score = playerInfo.Score;

            pack.PlayerPack.Add(playerPack);

            client?.CurrentRoom!.Broadcast(null, pack, server!);
        }
    }
}
