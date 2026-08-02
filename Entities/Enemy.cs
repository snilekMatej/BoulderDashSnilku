using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BoulderDashSnilku.Library;
using BoulderDashSnilku.World;
using BoulderDashSnilku.Simulation;

namespace BoulderDashSnilku.Entities
{
    public abstract class Enemy : Entity
    {
        public Direction Direction { get; set; }
        public int MoveTimer { get; set; }

        protected override ExplosionResult DeathResult => ExplosionResult.Gems;

        protected Enemy(int x, int y) : base(x, y)
        {
            Direction = Direction.Up;
            MoveTimer = 0;
        }

        public abstract Direction GetPreferredDirection();
        public abstract Direction GetBlockedTurn();
    }
}
