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
        public EntityManager EntityManager { get; }

        public LoadedLevel(GameWorld world, Player player, LevelState levelState, EntityManager entityManager)
        {
            World = world;
            Player = player;
            LevelState = levelState;
            EntityManager = entityManager;
        }
    }
}
