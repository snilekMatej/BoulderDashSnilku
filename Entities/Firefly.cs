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
    public class Firefly : Entity
    {
        public Direction Direction { get; set; }
        public int MoveTimer { get; set; } = 0;
        public Firefly(int x, int y) : base(x, y)
        {
            Direction = Direction.Up;
        }

        public override void Kill(GameWorld world, EntityManager entityManager, ExplosionLogic explosionLogic)
        {
            if (!IsAlive)
            {
                return;
            }
            IsAlive = false;

            explosionLogic.Explode(this, world, entityManager, ExplosionResult.Gems);
        }
    }
}
