namespace BoulderDashSnilku.Core {
    /// <summary>
    /// Current state of an active level.
    /// Controlls delays before gameplay and after player deaths.
    /// </summary>
    public enum GameplayState {
        WaitingToStart,
        Playing,
        WaitingToRestart
    }
}
