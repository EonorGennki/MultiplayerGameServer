using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerGameServer.Logic.Interface
{
    public interface IUserService
    {
        string GetUsername(int userId);
    }
}
