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

        //通用错误码
        UnknownError = 1000, //未知错误
        AlreadyExists = 1001, //已存在
        NotFound = 1002, //不存在

        //用户类错误码
        DatabaseError = 2001, //数据库错误
        InvalidUsernameFormat = 2002, //用户名格式错误
        InvalidPasswordFormat = 2003, //密码格式错误
        InvalidUsername = 2004, //用户名
        InvalidPassword = 2005, //密码

        //房间类错误码
        GameAlreadyStarted = 3001, //游戏已开始
        RoomIsFull = 3002 //房间已满
    }
}
