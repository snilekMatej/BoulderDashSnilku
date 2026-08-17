using BoulderDashSnilku.Library;

namespace BoulderDashSnilku.Entities {
    /// <summary>
    /// Enemy that follows walls by preferring right turns.
    /// Produces gems when destroyed.
    /// </summary>
    public class Butterfly : Enemy {
        public Butterfly(int x, int y) : base(x, y) { }

        public override Direction GetPreferredDirection() {
            return Direction.TurnRight();
        }

        public override Direction GetBlockedTurn() {
            return Direction.TurnLeft();
        }
    }
}
