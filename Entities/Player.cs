using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoulderDashSnilku.Entities
{
    public class Player
    {
        public int x { get; private set; }
        public int y { get; private set; }

        public Player(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public void MoveTo(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

    }
}
