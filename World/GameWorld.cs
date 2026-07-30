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
            Height = 22;
            Grid = new Tile[Width, Height];
        }
    }
}
