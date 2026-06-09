using MultiplayerGameServer.Network;
using MultiplayerGameServer.Service;
using SocketGameProtocal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if (userService.Register(username, password))
            {
                pack.ReturnCode = ReturnCode.Success;
            }
            else
            {
                pack.ReturnCode = ReturnCode.Failure;
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
            if (userService.Login(username, password))
            {
                pack.ReturnCode = ReturnCode.Success;
            }
            else
            {
                pack.ReturnCode = ReturnCode.Failure;
            }

                return pack;
        }
    }

}
