using MultiplayerGameServer.DAO;

namespace MultiplayerGameServer.Service
{
    internal class ServiceGroup
    {
        public UserService userService { get; }
        public RoomService roomService { get; }

        public RoomService RoomService
        {
            get { return roomService; }
        }

        public ServiceGroup(Database database, List<Room> roomList)
        {
            userService = new UserService(database);
            roomService = new RoomService(roomList);
        }
    }
}
