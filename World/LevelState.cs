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

        public int RequiredGems { get; set; }
        public int GemValue { get; set; }
        public int CollectedGems { get; set; }
        public int TimeLeft { get; set; }

        public bool IsExitOpen { get; private set; }
        public bool IsCompleted { get; private set; }

        public LevelState(int exitX, int exitY, int requiredGems, int gemValue, int timeLeft)
        {
            this.exitX = exitX;
            this.exitY = exitY;
            RequiredGems = requiredGems;
            GemValue = gemValue;
            TimeLeft = timeLeft;
        }
        public void CollectGem(GameWorld world)
        {
            CollectedGems++;

            if (!IsExitOpen && CollectedGems >= RequiredGems)
            {
                OpenExit(world);
            }
        }
        private void OpenExit(GameWorld world)
        {
            world.Grid[exitX, exitY] = Tile.Exit;
            IsExitOpen = true;
        }
        public void CompleteLevel()
        {
            IsCompleted = true;
        }
    }
}
