using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BoulderDashSnilku.Library;
using BoulderDashSnilku.Simulation;
using BoulderDashSnilku.World;

namespace BoulderDashSnilku.Entities
{
    public class Butterfly : Enemy
    {
        public Butterfly(int x, int y) : base(x, y) { }

        public override Direction GetPreferredDirection()
        {
            return Direction.TurnRight();
        }

        public override Direction GetBlockedTurn()
        {
            return Direction.TurnLeft();
        }

        public override void Kill(GameWorld world, EntityManager entityManager, ExplosionLogic explosionLogic)
        {
            if (IsAlive)
            {
                IsAlive = false;

                explosionLogic.Explode(this, world, entityManager, ExplosionResult.Gems);
            }
        }
    }
}
