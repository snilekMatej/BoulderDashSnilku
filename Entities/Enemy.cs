using BoulderDashSnilku.Library;
using BoulderDashSnilku.Simulation;

namespace BoulderDashSnilku.Entities {
    /// <summary>
    /// Base class containing behaviour of all enemy types.
    /// </summary>
    public abstract class Enemy : Entity {
        public Direction Direction { get; set; }
        public int MoveTimer { get; set; }
        protected override ExplosionResult DeathResult => ExplosionResult.Gems;

        protected Enemy(int x, int y) : base(x, y) {
            Direction = Direction.Up;
            MoveTimer = 0;
        }

        /// <summary>
        /// Get direction the enemy tries before continuing straight.
        /// </summary>
        /// <returns>First tried enemy direction.</returns>
        public abstract Direction GetPreferredDirection();

        /// <summary>
        /// Get direction the enemy turns to when its path is blocked.
        /// </summary>
        /// <returns>Direction that emeny turns to.</returns>
        public abstract Direction GetBlockedTurn();
    }
}
