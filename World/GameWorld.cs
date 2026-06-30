using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoulderDashSnilku.World
{
    public class GameWorld
    {
        public int Width;
        public int Height;

        public Tile[,] Grid;

        public GameWorld()
        {
            Width = 40;
            Height = 28;

            Grid = new Tile[Width, Height];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Grid[x, y] = Tile.Dirt;
                }
            }
            Grid[5, 5] = Tile.Boulder;
            Grid[6, 5] = Tile.Gem;
            Grid[6, 7] = Tile.Gem;
            Grid[6, 9] = Tile.Gem;
            Grid[6, 11] = Tile.Gem;
            Grid[7, 5] = Tile.Border;
            Grid[8, 5] = Tile.Wall;
        }
    }
}
