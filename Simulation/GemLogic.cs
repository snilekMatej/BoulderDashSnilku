using BoulderDashSnilku.Entities;
using BoulderDashSnilku.World;

namespace BoulderDashSnilku.Simulation {
    /// <summary>
    /// Applies fallingObject behaviour to gems.
    /// </summary>
    public class GemLogic {
        private readonly FallingObjectLogic fallingObjectLogic = new FallingObjectLogic();

        public void Update(GameWorld world, EntityManager entityManager,
            ExplosionLogic explosionLogic, int x, int y, bool gemFalling,
            bool[,] nextFallingObjects, bool[,] processedObjects) {
            fallingObjectLogic.Update(Tile.Gem, world, entityManager,
                explosionLogic, x, y, gemFalling,
                nextFallingObjects, processedObjects);
        }
    }
}
