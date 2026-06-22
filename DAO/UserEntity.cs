namespace MultiplayerGameServer.DAO
{
    public class UserEntity
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Salt { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime SignUpDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime UpdateTime { get; set; }

        public virtual ICollection<PlayerEntity> Players { get; set; } = new List<PlayerEntity>();
    }
}
