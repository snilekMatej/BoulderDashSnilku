namespace BoulderDashSnilku.Core {
    /// <summary>
    /// Storage for player data that are uneffected by loading levels.
    /// Contains information about current score and remaining lives.
    /// </summary>
    public class GameSession {
        public int Score { get; set; } = 0;
        public int PlayreLives { get; set; } = 3;
    }
}
