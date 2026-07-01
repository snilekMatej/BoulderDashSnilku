using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoulderDashSnilku.World;

namespace BoulderDashSnilku.Simulation
{
    public class GemLogic
    {
        public void Update(GameWorld world, int x, int y)
        {
            if (world.Grid[x, y + 1] == Tile.Empty)
            {
                world.Grid[x, y + 1] = Tile.Gem;
                world.Grid[x, y] = Tile.Empty;
            }
        }
    }
}
