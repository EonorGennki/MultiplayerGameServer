using MultiplayerGameServer.Network;
using SocketGameProtocal;

namespace MultiplayerGameServer.Controllers
{
    abstract class BaseController
    {
        protected RequestCode requestCode = RequestCode.ReqNone;

        public RequestCode RequestCode
        {
            get { return requestCode; }
        }
    }
}
