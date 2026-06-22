using MultiplayerGameServer.Logic.Service;
using MultiplayerGameServer.Network;
using SocketGameProtocal;

namespace MultiplayerGameServer.Controllers
{
    internal class UserController : BaseController
    {
        private UserService userService;

        public UserController(UserService userService)
        {
            requestCode = RequestCode.User;
            this.userService = userService;
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        /// <returns></returns>
        public MainPack Register(Server server, Client client, MainPack pack)
        {
            string username = pack.AuthPack.Username;
            string password = pack.AuthPack.Password;

            ServiceResult result = userService.Register(username, password);
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
        /// 登录
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        /// <returns></returns>
        public MainPack Login(Server server, Client client, MainPack pack)
        {
            string username = pack.AuthPack.Username;
            string password = pack.AuthPack.Password;

            ServiceResult result = userService.Login(username, password);
            if (result.IsSuccess)
            {
                UserData userData = result.GetData<UserData>()!;
                client.UserId = userData.UserId;
                client.PlayerId = userData.PlayerId;

                PlayerPack playerPack = new PlayerPack();
                playerPack.PlayerId = userData.PlayerId;

                pack.ReturnCode = ReturnCode.Success;
                pack.PlayerPack.Add(playerPack);
            }
            else
            {
                pack.ReturnCode = ReturnCode.Failure;
                pack.ErrorCode = (ErrorCode)result.ErrorCode;
            }

            return pack;
        }
    }

}
