using MultiplayerGameServer.Network;
using SocketGameProtocal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerGameServer.Logic
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
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        /// <returns></returns>
        public MainPack SignUp(Server server, Client client, MainPack pack)
        {
            if (server.userDatabase.SignUp(pack))
            {
                pack.ReturnCode = ReturnCode.Succeeded;
            }
            else
            {
                pack.ReturnCode = ReturnCode.Failed;
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
            return pack;
        }
    }

}
