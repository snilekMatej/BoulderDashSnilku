using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BoulderDashSnilku.Entities;
using BoulderDashSnilku.World;

namespace BoulderDashSnilku.Simulation
{
    public class ExplosionLogic
    {
        public void Explode(Entity source, GameWorld world, EntityManager entityManager, ExplosionResult result)
        {
            int centerX = source.x;
            int centerY = source.y;

            entityManager.Remove(source);

            for (int offsetY = -1 ; offsetY <= 1; offsetY++)
            {
                for (int offsetX = -1; offsetX <= 1; offsetX++)
                {
                    int x = centerX + offsetX;
                    int y = centerY + offsetY;

                    if (!IsInsideWorld(world, x, y))
                    {
                        continue;
                    }
                    DestroyEntityAt(source, x, y, world, entityManager);
                    ReplaceTile(world, x, y, result);
                }
            }
        }

        private void DestroyEntityAt(Entity source, int x, int y, GameWorld world, EntityManager entityManager)
        {
            Entity? target = entityManager.GetEntityAt(x, y);

            if (target == null || target == source)
            {
                return;
            }
            target.Kill(world, entityManager, this);
        }

        private void ReplaceTile(GameWorld world, int x, int y, ExplosionResult result)
        {
            if (world.Grid[x, y] == Tile.Border || world.Grid[x, y] == Tile.Exit)
            {
                return;
            }

            world.Grid[x, y] = result switch
            {
                ExplosionResult.Gems => Tile.Gem,
                _ => Tile.Empty
            };
        }

        private bool IsInsideWorld(GameWorld world, int x, int y)
        {
            return x >= 0 && x < world.Width && y >= 0 && y < world.Height;
        }
    }
}
