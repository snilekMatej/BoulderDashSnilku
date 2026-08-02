using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BoulderDashSnilku.Simulation;
using BoulderDashSnilku.World;

namespace BoulderDashSnilku.Entities
{
    public class Player : Entity
    {
        protected override ExplosionResult DeathResult => ExplosionResult.Empty;

        public Player(int x, int y) : base(x, y) { }
    }
}
