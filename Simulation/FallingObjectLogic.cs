using BoulderDashSnilku.Entities;
using BoulderDashSnilku.World;

namespace BoulderDashSnilku.Simulation {
    /// <summary>
    /// Contains movement for boulders and gems.
    /// Handles falling, crushing entities and rolling off.
    /// </summary>
    class FallingObjectLogic {
        /// <summary>
        /// Update one falling object depending on space bellow it.
        /// Object tries to fall or roll off some surfaces.
        /// </summary>
        public void Update(Tile objectTile, GameWorld world, EntityManager entityManager,
            ExplosionLogic explosionLogic, int x, int y, bool wasFalling,
            bool[,] nextFallingObjects, bool[,] processedObjects) {
            nextFallingObjects[x, y] = false;
            int bellowX = x;
            int bellowY = y + 1;
            Tile tileBelow = world.Grid[bellowX, bellowY];
            Entity entityBelow = entityManager.GetEntityAt(bellowX, bellowY);
            if (tileBelow == Tile.Empty)
                UpdateVerticalMovement(objectTile, world, entityManager,
                    explosionLogic, x, y, wasFalling, entityBelow,
                    nextFallingObjects, processedObjects);
            else if (CanRollOff(tileBelow)) {
                bool rolled = TryRoll(objectTile, world, entityManager, x, y,
                    1, nextFallingObjects, processedObjects);
                if (!rolled) TryRoll(objectTile, world, entityManager, x, y, 
                    -1, nextFallingObjects, processedObjects);
            }
        }

        /// <summary>
        /// Handle starting, continuing or completing falling behaviour.
        /// If already falling then kill entities on inpact.
        private void UpdateVerticalMovement(Tile objectTile, GameWorld world,
            EntityManager entityManager, ExplosionLogic explosionLogic,
            int x, int y, bool wasFalling, Entity entityBelow,
            bool[,] nextFallingObjects, bool[,] processedObjects) {
            int belowX = x;
            int belowY = y + 1;
            if (!wasFalling && entityBelow == null)
                nextFallingObjects[x, y] = true;
            else if (wasFalling && entityBelow != null) {
                Entity entity = entityManager.GetEntityAt(belowX, belowY);
                entity.Kill(world, entityManager, explosionLogic);
            } else if (wasFalling && entityBelow == null) {
                MoveObject(objectTile, world, x, y, belowX, belowY);
                nextFallingObjects[belowX, belowY] = true;
                processedObjects[belowX, belowY] = true;
            }
        }

        private bool CanRollOff(Tile tileBelow) {
            return tileBelow is Tile.Border or
                Tile.Wall or Tile.Boulder or Tile.Gem;
        }

        /// <summary>
        /// Attempt to roll off to the side if that tile and the tile bellow that tile are empty.
        /// Successful roll marks new position as processed and falling.
        /// </summary>
        /// <returns>True -> roll off was successful.</returns>
        private bool TryRoll(Tile objectTile, GameWorld world,
            EntityManager entityManager, int x, int y, int directionX,
            bool[,] nextFallingObjects, bool[,] processedObjects) {
            int sideX = x + directionX;
            int sideY = y;
            int diagonalX = sideX;
            int diagonalY = sideY + 1;
            bool canRoll = world.IsInBounds(sideX, sideY) &&
                world.IsInBounds(diagonalX, diagonalY);
            if (canRoll) canRoll =
                IsTrulyEmpty(world, entityManager, sideX, sideY) &&
                IsTrulyEmpty(world, entityManager, diagonalX, diagonalY);
            if (canRoll) {
                MoveObject(objectTile, world, x, y, sideX, sideY);
                nextFallingObjects[sideX, sideY] = true;
                processedObjects[sideX, sideY] = true;
            }
            return canRoll;
        }

        private static void MoveObject(Tile objectTile, GameWorld world,
            int oldX, int oldY, int newX, int newY) {
            world.Grid[newX, newY] = objectTile;
            world.Grid[oldX, oldY] = Tile.Empty;
        }

        private static bool IsTrulyEmpty(GameWorld world,
            EntityManager entityManager, int x, int y) {
            return world.Grid[x, y] == Tile.Empty && !entityManager.HasEntityAt(x, y);
        }
    }
}
