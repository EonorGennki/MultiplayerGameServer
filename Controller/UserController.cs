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
        /// <param name="_server"></param>
        /// <param name="_client"></param>
        /// <param name="_pack"></param>
        /// <returns></returns>
        public MainPack SignUp(Server _server, Client _client, MainPack _pack)
        {
            if (_server.SignUp(_client, _pack))
            {
                _pack.ReturnCode = ReturnCode.Succeeded;
            }
            else
            {
                _pack.ReturnCode = ReturnCode.Failed;
            }

            return _pack;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="_server"></param>
        /// <param name="_client"></param>
        /// <param name="_pack"></param>
        /// <returns></returns>
        public MainPack Login(Server _server, Client _client, MainPack _pack)
        {
            return null;
        }
    }

}
