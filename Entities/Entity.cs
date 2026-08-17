using BoulderDashSnilku.Simulation;
using BoulderDashSnilku.World;

namespace BoulderDashSnilku.Entities {
    /// <summary>
    /// Base class for all movable entities separated from world map Tiles.
    /// Stores position, IsAlive state, provides common movement and death behaviour.
    /// </summary>
    public abstract class Entity {
        public int x { get; protected set; }
        public int y { get; protected set; }
        public bool IsAlive { get; protected set; } = true;
        protected abstract ExplosionResult DeathResult { get; }

        protected Entity(int x, int y) {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Changes entity's position to new position.
        /// </summary>
        /// <param name="x">New hosizontal tile coordinate.</param>
        /// <param name="y">New vertical tile coordinate.</param>
        public void MoveTo(int x, int y) {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Kills the entity when it IsAlive and trigers it's death explosion.
        /// Resulting explosion contents are determined by entity's DeathResult.
        /// </summary>
        // Should this contain param  ? ? ?
        public void Kill(
            GameWorld world, EntityManager entityManager, ExplosionLogic explosionLogic) {
            if (IsAlive) {
                IsAlive = false;
                explosionLogic.Explode(this, world, entityManager, DeathResult);
            }
        }
    }
}
