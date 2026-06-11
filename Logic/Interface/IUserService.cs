using MultiplayerGameServer.Logic.Service;

namespace MultiplayerGameServer.Logic.Interface
{
    public interface IUserService
    {
        PlayerInfo GetPlayerInfo(int userId);
    }
}
