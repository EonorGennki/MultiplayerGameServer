using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerGameServer.DAO
{
    public class UserEntity
    {
        public int UserId {  get; set; }
        public string UserName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Salt { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime SignUpDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
