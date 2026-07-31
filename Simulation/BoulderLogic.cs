using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoulderDashSnilku.World;
using BoulderDashSnilku.Entities;

namespace BoulderDashSnilku.Simulation
{
    public class BoulderLogic
    {
        public void Update(GameWorld world, EntityManager entityManager, ExplosionLogic explosionLogic, int x, int y, bool boulderFalling, bool[,] nextFallingObjects)
        {
            bool entityBellow = entityManager.HasEntityAt(x, y + 1);
            Tile tileBellow = world.Grid[x, y + 1];
            nextFallingObjects[x, y] = false;
            if (tileBellow == Tile.Empty)
            {
                if (!entityBellow && !boulderFalling)
                {
                    nextFallingObjects[x, y] = true;
                }
                else if (entityBellow && boulderFalling)
                {
                    Entity entity = entityManager.GetEntityAt(x, y + 1);
                    entity.Kill(world, entityManager, explosionLogic);
                    return;
                }
                else if (!entityBellow)
                {
                    world.Grid[x, y + 1] = Tile.Boulder;
                    nextFallingObjects[x, y + 1] = true;
                    world.Grid[x, y] = Tile.Empty;
                }
            }
            else if (tileBellow == Tile.Wall || tileBellow == Tile.Boulder || tileBellow == Tile.Gem)
            {
                if (x < world.Width - 1 && world.Grid[x + 1, y] == Tile.Empty && world.Grid[x + 1, y + 1] == Tile.Empty && !entityManager.HasEntityAt(x + 1, y) && !entityManager.HasEntityAt(x + 1, y + 1))
                {
                    world.Grid[x + 1, y] = Tile.Boulder;
                    nextFallingObjects[x + 1, y] = false;
                    world.Grid[x, y] = Tile.Empty;
                }
                else if (x > 0 && world.Grid[x - 1, y] == Tile.Empty && world.Grid[x - 1, y + 1] == Tile.Empty && !entityManager.HasEntityAt(x - 1, y) && !entityManager.HasEntityAt(x - 1, y + 1))
                {
                    world.Grid[x - 1, y] = Tile.Boulder;
                    nextFallingObjects[x - 1, y] = true;
                    world.Grid[x, y] = Tile.Empty;
                }
            }
        }
    }
}
