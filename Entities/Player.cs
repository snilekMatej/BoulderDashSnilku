using BoulderDashSnilku.Simulation;

namespace BoulderDashSnilku.Entities {
    /// <summary>
    /// Represents entity controlled by player.
    /// Upon death player leaves empty tiles behind.
    /// </summary>
    public class Player : Entity {
        protected override ExplosionResult DeathResult => ExplosionResult.Empty;
        public Player(int x, int y) : base(x, y) { }
    }
}
