namespace MultiplayerGameServer.Logic.Service
{
    internal class RoomInfo
    {
        public string RoomName { get; }
        public int CurrentNum { get; set; }
        public int MaxNum { get; }
        public int State { get; set; }

        public RoomInfo(string roomName, int maxNum, int state)
        {
            RoomName = roomName;
            MaxNum = maxNum;
            State = state;
        }
    }
}
