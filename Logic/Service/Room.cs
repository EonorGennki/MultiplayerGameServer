using MultiplayerGameServer.DAO;
using MultiplayerGameServer.Network;

namespace MultiplayerGameServer.Logic.Service
{
    internal class Room
    {
        public RoomInfo roomInfo;

        private List<Client> clientList = new List<Client>(); //房间内所有客户端
        public List<Client> ClientList
        {
            get { return clientList; } 
        }

        /// <summary>
        /// 创建房间
        /// </summary>
        /// <param name="client"></param>
        /// <param name="roomName"></param>
        /// <param name="maxNum"></param>
        /// <param name="state"></param>
        public Room(Client client, RoomInfo roomInfo)
        {
            this.roomInfo = roomInfo;
            clientList.Add(client);
            this.roomInfo.currentNum = clientList.Count();
        }

        /// <summary>
        /// 搜索房间
        /// </summary>
        /// <param name="roomName"></param>
        /// <param name="maxNum"></param>
        /// <param name="state"></param>
        public Room(RoomInfo roomInfo)
        {
            this.roomInfo = roomInfo;
        }
    }
}
