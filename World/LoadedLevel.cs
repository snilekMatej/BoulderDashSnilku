using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BoulderDashSnilku.Entities;

namespace BoulderDashSnilku.World
{
    public class LoadedLevel
    {
        public GameWorld World { get; }
        public Player Player { get; }
        public LevelState LevelState { get; }

        public LoadedLevel(GameWorld world, Player player, LevelState levelState)
        {
            World = world;
            Player = player;
            LevelState = levelState;
        }
    }
}
