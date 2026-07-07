using MultiplayerGameServer.DAO;

namespace MultiplayerGameServer.Logic.Service
{
    internal class ServiceGroup
    {
        public UserService UserService { get; }
        public RoomService RoomService { get; }
        public GameService GameService { get; }

        public ServiceGroup(Database database, List<Room> roomList)
        {
            UserService = new UserService(database);
            RoomService = new RoomService(UserService, roomList);
            GameService = new GameService(roomList);
        }
    }
}
