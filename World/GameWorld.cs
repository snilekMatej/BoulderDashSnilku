using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoulderDashSnilku.World
{
    public class GameWorld
    {
        public const int DefaultWidth = 40;
        public const int DefaultHeight = 22;

        public int Width { get; } = DefaultWidth;
        public int Height { get; } = DefaultHeight;

        public Tile[,] Grid;

        public GameWorld()
        {
            Grid = new Tile[Width, Height];
        }

        public bool IsInBounds(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }
    }
}
