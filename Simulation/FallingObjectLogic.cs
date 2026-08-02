using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoulderDashSnilku.Entities;
using BoulderDashSnilku.World;

namespace BoulderDashSnilku.Simulation
{
    class FallingObjectLogic
    {
        public void Update(Tile objectTile, GameWorld world, EntityManager entityManager, ExplosionLogic explosionLogic, int x, int y, bool wasFalling, bool[,] nextFallingObjects, bool[,] processedObjects)
        {
            nextFallingObjects[x, y] = false;
            int bellowX = x;
            int bellowY = y + 1;
            Tile tileBelow = world.Grid[bellowX, bellowY];
            Entity entityBelow = entityManager.GetEntityAt(bellowX, bellowY);
            
            if (tileBelow == Tile.Empty)
            {
                UpdateVerticalMovement(objectTile, world, entityManager, explosionLogic, x, y, wasFalling, entityBelow, nextFallingObjects, processedObjects);
            }
            else if (CanRollOff(tileBelow))
            {
                if (TryRoll(objectTile, world, entityManager, x, y, 1, nextFallingObjects, processedObjects))
                {
                    return;
                }
                    TryRoll(objectTile, world, entityManager, x, y, -1, nextFallingObjects, processedObjects);
            }
        }

        private void UpdateVerticalMovement(Tile objectTile, GameWorld world, EntityManager entityManager, ExplosionLogic explosionLogic, int x, int y, bool wasFalling, Entity entityBelow, bool[,] nextFallingObjects, bool[,] processedObjects)
        {
            int belowX = x;
            int belowY = y + 1;

            if (!wasFalling && entityBelow == null)
            {
                nextFallingObjects[x, y] = true;
            }
            else if (wasFalling && entityBelow != null)
            {
                Entity entity = entityManager.GetEntityAt(belowX, belowY);
                entity.Kill(world, entityManager, explosionLogic);
            }
            else if (wasFalling && entityBelow == null)
            {
                MoveObject(objectTile, world, x, y, belowX, belowY);
                
                nextFallingObjects[belowX, belowY] = true;
                processedObjects[belowX, belowY] = true;
            }
        }

        private bool CanRollOff(Tile tileBelow)
        {
            return tileBelow == Tile.Border || tileBelow == Tile.Wall || tileBelow == Tile.Boulder || tileBelow == Tile.Gem;
        }

        private bool TryRoll(Tile objectTile, GameWorld world, EntityManager entityManager, int x, int y, int directionX, bool[,] nextFallingObjects, bool[,] processedObjects)
        {
            int sideX = x + directionX;
            int sideY = y;

            int diagonalX = sideX;
            int diagonalY = sideY + 1;

            if (!world.IsInBounds(sideX, sideY) || !world.IsInBounds(diagonalX, diagonalY))
            {
                return false;
            }
            else if (!IsTrulyEmpty(world, entityManager, sideX, sideY) || !IsTrulyEmpty(world, entityManager, diagonalX, diagonalY))
            {
                return false;
            }
            MoveObject(objectTile, world, x, y, sideX, sideY);

            nextFallingObjects[sideX, sideY] = true;
            processedObjects[sideX, sideY] = true;

            return true;
        }

        private static void MoveObject(Tile objectTile, GameWorld world, int oldX, int oldY, int newX, int newY)
        {
            world.Grid[newX, newY] = objectTile;
            world.Grid[oldX, oldY] = Tile.Empty;
        }

        private static bool IsTrulyEmpty(GameWorld world, EntityManager entityManager, int x, int y)
        {
            return world.Grid[x, y] == Tile.Empty && !entityManager.HasEntityAt(x, y);
        }
    }
}
