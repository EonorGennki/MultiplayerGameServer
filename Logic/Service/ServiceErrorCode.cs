using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerGameServer.Logic.Service
{
    public enum ServiceErrorCode
    {
        ErrorNone = 0,

        UnknownError = 1000,
        AlreadyExists = 1001,
        NotFound = 1002,

        DatabaseError = 2001,
        InvalidUsernameFormat = 2002,
        InvalidPasswordFormat = 2003,
        InvalidUsername = 2004,
        InvalidPassword = 2005,
    }
}
