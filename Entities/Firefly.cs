using BoulderDashSnilku.Library;

namespace BoulderDashSnilku.Entities {
    /// <summary>
    /// Enemy that follows walls by preferring left turns.
    /// Produces gems, when destroyed.
    /// </summary>
    public class Firefly : Enemy {
        public Firefly(int x, int y) : base(x, y) { }

        public override Direction GetPreferredDirection() {
            return Direction.TurnLeft();
        }

        public override Direction GetBlockedTurn() {
            return Direction.TurnRight();
        }
    }
}
