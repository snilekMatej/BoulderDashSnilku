using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BoulderDashSnilku.Library;

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
    }
}
