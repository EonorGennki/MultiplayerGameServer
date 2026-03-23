using MultiplayerGameServer.Servers;
using SocketGameProtocal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerGameServer.Controller
{
    internal class UserController : BaseController
    {
        public UserController()
        {
            requestCode = RequestCode.User;
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <returns></returns>
        public MainPack SignUp(Server server, Client client, MainPack pack)
        {
            return null;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        public MainPack Login(Server server, Client client, MainPack pack)
        {
            return null;
        }
    }

}
