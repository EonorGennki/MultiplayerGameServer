namespace MultiplayerGameServer.Logic.Service
{
    public class PlayerInfo
    {
        public long PlayerId { get; }
        public string PlayerName { get; }
        public bool IsReady { get; private set; } = false;
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Score {  get; set; }
        public float Pos_X { get; set; }
        public float Pos_Y { get; set; }
        public float Rot_Z { get; set; }
        public float Gun_Rot_Z { get; set; }

        public PlayerInfo(long playerId, string playerName)
        {
            this.PlayerId = playerId;
            this.PlayerName = playerName;
        }

        public bool ToggleIsReady(bool isReady) => this.IsReady = !isReady;
        public void SetIsReady(bool isReady) => this.IsReady = isReady;
    }
}
