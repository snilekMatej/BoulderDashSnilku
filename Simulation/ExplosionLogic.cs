using BoulderDashSnilku.Entities;
using BoulderDashSnilku.World;

namespace BoulderDashSnilku.Simulation {
    /// <summary>
    /// Handles 3x3 explosion produced when entity dies.
    /// Explosions destroy affected entities and replace surounding terain with requested result.
    /// </summary>
    public class ExplosionLogic {

        /// <summary>
        /// Apply exlposion centered on given entity target.
        /// Removes that entity from EntityManager.
        /// Esplosions don't affect Border and exit tiles.
        /// </summary>
        public void Explode(Entity source, GameWorld world,
            EntityManager entityManager, ExplosionResult result) {
            int centerX = source.x;
            int centerY = source.y;
            entityManager.Remove(source);
            for (int offsetY = -1 ; offsetY <= 1; offsetY++) {
                for (int offsetX = -1; offsetX <= 1; offsetX++) {
                    int x = centerX + offsetX;
                    int y = centerY + offsetY;
                    if (world.IsInBounds(x, y)) {
                        DestroyEntityAt(source, x, y, world, entityManager);
                        ReplaceTile(world, x, y, result);
                    }
                }
            }
        }

        /// <summary>
        /// Kill entity in explosion radius.
        /// Source tile is ignored.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="world"></param>
        /// <param name="entityManager"></param>
        private void DestroyEntityAt(Entity source, int x, int y,
            GameWorld world, EntityManager entityManager) {
            Entity? target = entityManager.GetEntityAt(x, y);
            if (target != null && target != source)
                target.Kill(world, entityManager, this);
        }

        /// <summary>
        /// Replace affected tile with explosion result.
        /// Border and Exit can't be destroied.
        /// </summary>
        private void ReplaceTile(GameWorld world, int x, int y,
            ExplosionResult result) {
            Tile tile = world.Grid[x, y];
            if (tile != Tile.Border && tile != Tile.Exit)
                world.Grid[x, y] = result switch {
                    ExplosionResult.Gems => Tile.Gem,
                    ExplosionResult.Empty => Tile.Empty,
                    _ => Tile.Empty
                };
        }
    }
}
