using BoulderDashSnilku.World;
using BoulderDashSnilku.Entities;

namespace BoulderDashSnilku.Simulation {
    /// <summary>
    /// Applies fallingObject behaviour to boulders.
    /// </summary>
    public class BoulderLogic {
        private readonly FallingObjectLogic fallingObjectLogic = new FallingObjectLogic();

        public void Update(GameWorld world, EntityManager entityManager,
            ExplosionLogic explosionLogic, int x, int y, bool boulderFalling,
            bool[,] nextFallingObjects, bool[,] processedObjects) {
            fallingObjectLogic.Update(Tile.Boulder, world, entityManager,
                explosionLogic, x, y, boulderFalling,
                nextFallingObjects, processedObjects);
        }
    }
}
