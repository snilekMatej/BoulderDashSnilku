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
        public void Update(Tile objectTile, GameWorld world, EntityManager entityManager, ExplosionLogic explosionLogic, int x, int y, bool wasFalling, bool[,] nextFallingObjects)
        {
            nextFallingObjects[x, y] = false;
            int bellowX = x;
            int bellowY = y + 1;
            Tile tileBelow = world.Grid[bellowX, bellowY];
            bool entityBelow = entityManager.HasEntityAt(bellowX, bellowY);
            
            if (tileBelow == Tile.Empty)
            {
                UpdateVerticalMovement(objectTile, world, entityManager, explosionLogic, x, y, wasFalling, entityBelow, nextFallingObjects);
            }
            else if (CanRollOff(tileBelow))
            {
                if (TryRoll(objectTile, world, entityManager, x, y, 1, nextFallingObjects))
                {
                    return;
                }
                else
                {
                    TryRoll(objectTile, world, entityManager, x, y, -1, nextFallingObjects);
                }
            }
        }

        private void UpdateVerticalMovement(Tile objectTile, GameWorld world, EntityManager entityManager, ExplosionLogic explosionLogic, int x, int y, bool wasFalling, bool entityBelow, bool[,] nextFallingObjects)
        {
            int bellowX = x;
            int bellowY = y + 1;

            if (!wasFalling && !entityBelow)
            {
                nextFallingObjects[x, y] = true;
            }
            else if (wasFalling && entityBelow)
            {
                Entity entity = entityManager.GetEntityAt(bellowX, bellowY);
                entity.Kill(world, entityManager, explosionLogic);
            }
            else if (wasFalling && !entityBelow)
            {
                MoveObject(objectTile, world, x, y, bellowX, bellowY);
                nextFallingObjects[bellowX, bellowY] = true;
            }
        }

        private bool CanRollOff(Tile tileBelow)
        {
            return tileBelow == Tile.Border || tileBelow == Tile.Wall || tileBelow == Tile.Boulder || tileBelow == Tile.Gem;
        }

        private bool TryRoll(Tile objectTile, GameWorld world, EntityManager entityManager, int x, int y, int directionX, bool[,] nextFallingObjects)
        {
            int sideX = x + directionX;
            int sideY = y;

            int diagonalX = sideX;
            int diagonalY = sideY + 1;

            if (!IsInsideWorld(world, sideX, sideY) || !IsInsideWorld(world, diagonalX, diagonalY))
            {
                return false;
            }
            else if (IsTrulyEmpty(world, entityManager, sideX, sideY) && IsTrulyEmpty(world, entityManager, diagonalX, diagonalY))
            {
                MoveObject(objectTile, world, x, y, sideX, sideY);
                nextFallingObjects[sideX, sideY] = false;
                return true;
            }
            else
            {
                return false;
            }
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

        private static bool IsInsideWorld(GameWorld world, int x, int y)
        {
            return x >= 0 && x < world.Width && y >= 0 && y < world.Height;
        }
    }
}
