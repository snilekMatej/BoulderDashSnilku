using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoulderDashSnilku.Entities
{
    public abstract class Entity
    {
        public int x { get; protected set; }
        public int y { get; protected set; }
        public bool IsAlive { get; protected set; } = true;

        protected Entity(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public void MoveTo(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public void Kill()
        {
            // This method can be used to remove the entity from the game world or mark it as dead.
            IsAlive = false;
        }
    }
}
