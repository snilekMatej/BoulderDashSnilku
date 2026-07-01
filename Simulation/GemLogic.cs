using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoulderDashSnilku.Entities;
using BoulderDashSnilku.World;

namespace BoulderDashSnilku.Simulation
{
    public class GemLogic
    {
        public void Update(GameWorld world, EntityManager entityManager, int x, int y, bool gemFalling, bool[,] nextFallingObjects)
        {
            bool entityBellow = entityManager.HasEntityAt(x, y + 1);
            nextFallingObjects[x, y] = false;
            if (world.Grid[x, y + 1] == Tile.Empty)
            {
                if (entityBellow && !gemFalling)
                {
                    return;
                }
                else if (entityBellow && gemFalling)
                {
                    Entity entity = entityManager.GetEntityAt(x, y + 1);
                    entity.Kill();
                    entityManager.Remove(entity);

                    world.Grid[x, y + 1] = Tile.Gem;
                    nextFallingObjects[x, y + 1] = true;
                    world.Grid[x, y] = Tile.Empty;
                }
                else if (!entityBellow)
                {
                    world.Grid[x, y + 1] = Tile.Gem;
                    nextFallingObjects[x, y + 1] = true;
                    world.Grid[x, y] = Tile.Empty;
                }
            }
        }
    }
}
