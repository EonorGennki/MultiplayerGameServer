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

        //用户类错误码
        DatabaseError = 2001, //数据库错误
        InvalidUsernameFormat = 2002, //用户名格式错误
        InvalidPasswordFormat = 2003, //密码格式错误
        InvalidUsername = 2004, //用户名无效
        InvalidPassword = 2005, //密码无效
        UserNotFound = 2006, //用户不存在
        UserAlreadyExists = 2007, //用户已存在

        //房间类错误码
        RoomNotFound = 3001, //房间不存在
        RoomAlreadyExists = 3002, //房间已存在
        GameAlreadyStarted = 3003, //游戏已开始
        RoomIsFull = 3004, //房间已满
        PlayerNotReady = 3005, //玩家未准备
    }
}
