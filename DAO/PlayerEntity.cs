using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerGameServer.DAO
{
    public class PlayerEntity
    {
        public long PlayerId { get; set; }
        public bool isActive { get; set; }
    }
}
