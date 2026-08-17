namespace BoulderDashSnilku.World {
    /// <summary>
    /// Stores tile grid representing current level with fixed proportions.
    /// Provides boundary checking for world coordinates.
    /// </summary>
    public class GameWorld {
        public const int DefaultWidth = 40;
        public const int DefaultHeight = 22;
        public int Width { get; } = DefaultWidth;
        public int Height { get; } = DefaultHeight;
        public Tile[,] Grid { get; }

        public GameWorld() {
            Grid = new Tile[Width, Height];
        }

        /// <summary>
        /// Check if given coordinates are inside world grid.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool IsInBounds(int x, int y) {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }
    }
}