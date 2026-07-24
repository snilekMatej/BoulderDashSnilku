using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoulderDashSnilku.Library;

namespace BoulderDashSnilku.World
{
    public class LevelState
    {
        private const int DefaultGemQuota = 20;

        private readonly int exitX;
        private readonly int exitY;

        public int CollectedGems { get; private set; }
        public int GemQuota { get; }

        public bool IsExitOpen { get; private set; }

        public LevelState(int exitX, int exitY)
        {
            this.exitX = exitX;
            this.exitY = exitY;

            GemQuota = DefaultGemQuota;
        }
        public void CollectGem(GameWorld world)
        {
            CollectedGems++;

            if (!IsExitOpen && CollectedGems >= GemQuota)
            {
                OpenExit(world);
            }
        }
        private void OpenExit(GameWorld world)
        {
            world.Grid[exitX, exitY] = Tile.Exit;
            IsExitOpen = true;
        }
    }
}
