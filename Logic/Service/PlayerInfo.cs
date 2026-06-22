namespace MultiplayerGameServer.Logic.Service
{
    public class PlayerInfo
    {
        public long playerId { get; }
        public string playerName { get; }
        public bool isReady { get; private set; } = false;

        public PlayerInfo(long playerId, string playerName)
        {
            this.playerId = playerId;
            this.playerName = playerName;
        }

        public bool ToggleIsReady(bool isReady) => this.isReady = !isReady;
        public void SetIsReady(bool isReady) => this.isReady = isReady;
    }
}
