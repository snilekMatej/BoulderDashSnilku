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
        private readonly FallingObjectLogic fallingObjectLogic = new FallingObjectLogic();

        public void Update(GameWorld world, EntityManager entityManager, ExplosionLogic explosionLogic, int x, int y, bool boulderFalling, bool[,] nextFallingObjects)
        {
            fallingObjectLogic.Update(Tile.Boulder, world, entityManager, explosionLogic, x, y, boulderFalling, nextFallingObjects);
        }
    }
}
