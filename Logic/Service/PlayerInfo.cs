using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerGameServer.Logic.Service
{
    public class PlayerInfo
    {
        public string playerName { get; }

        public PlayerInfo(string playerName)
        {
            this.playerName = playerName;
        }
    }
}
