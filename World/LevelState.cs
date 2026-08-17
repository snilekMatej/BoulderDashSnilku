namespace BoulderDashSnilku.World {
    /// <summary>
    /// Stores progress and settings specific for current level.
    /// Tracks gems, time, exit state and level completion.
    /// </summary>
    public class LevelState {
        private readonly int exitX;
        private readonly int exitY;
        public int RequiredGems { get; set; }
        public int GemValue { get; set; }
        public int CollectedGems { get; private set; }
        public int TimeLeft { get; set; }
        public bool IsExitOpen { get; private set; }
        public bool IsCompleted { get; private set; }

        public LevelState(int exitX, int exitY, int requiredGems,
            int gemValue, int timeLeft) {
            this.exitX = exitX;
            this.exitY = exitY;
            RequiredGems = requiredGems;
            GemValue = gemValue;
            TimeLeft = timeLeft;
        }

        /// <summary>
        /// Record one collected gem and open exit once the required quota is reached.
        /// </summary>
        /// <param name="world">World containing level exit tile.</param>
        public void CollectGem(GameWorld world) {
            CollectedGems++;
            if (!IsExitOpen && CollectedGems >= RequiredGems) OpenExit(world);
        }

        /// <summary>
        /// Change border tile to active exit tile at exit position.
        /// </summary>
        private void OpenExit(GameWorld world) {
            world.Grid[exitX, exitY] = Tile.Exit;
            IsExitOpen = true;
        }

        /// <summary>
        /// Mark the level as completed after player enters open exit.
        /// </summary>
        public void CompleteLevel() {
            IsCompleted = true;
        }
    }
}
