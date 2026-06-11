namespace MultiplayerGameServer.Logic.Service
{
    internal class RoomInfo
    {
        public string RoomName { get; }
        public int currentNum { get; set; }
        public int MaxNum { get; }
        public int State { get; }

        public RoomInfo(string roomName, int maxNum, int state)
        {
            RoomName = roomName;
            MaxNum = maxNum;
            State = state;
        }
    }
}
