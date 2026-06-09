using SocketGameProtocal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
