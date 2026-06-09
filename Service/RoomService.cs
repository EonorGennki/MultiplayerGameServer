using MultiplayerGameServer.Network;

namespace MultiplayerGameServer.Service
{
    internal class RoomService
    {
        private List<Room> roomList;

        public RoomService(List<Room> roomList)
        {
            this.roomList = roomList;
        }

        public bool CreateRoom(Client client, string roomName, int maxNum, string state)
        {
            try
            {
                Room room = new Room(client, roomName, maxNum, state);
                roomList.Add(room);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<Room> SearchRoom()
        {
            List<Room> rooms = new List<Room>();

            foreach (Room room in roomList)
            {
                Room newRoom = new Room(room.RoomName, room.MaxNum, room.State);
                rooms.Add(room);
            }

            return rooms;
        }
    }
}
