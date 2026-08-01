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
        private readonly FallingObjectLogic fallingObjectLogic = new FallingObjectLogic();

        public void Update(GameWorld world, EntityManager entityManager, ExplosionLogic explosionLogic, int x, int y, bool gemFalling, bool[,] nextFallingObjects, bool[,] processedObjects)
        {
            fallingObjectLogic.Update(Tile.Gem, world, entityManager, explosionLogic, x, y, gemFalling, nextFallingObjects, processedObjects);
        }
    }
}
