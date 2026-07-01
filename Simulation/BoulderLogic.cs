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
        public void Update(GameWorld world, EntityManager entityManager, int x, int y)
        {
            bool entityBellow = entityManager.HasEntityAt(x, y + 1);

            if (world.Grid[x, y + 1] == Tile.Empty )
            {
                world.Grid[x, y + 1] = Tile.Boulder;
                world.Grid[x, y] = Tile.Empty;
            }
        }
    }
}
