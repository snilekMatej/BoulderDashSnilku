using BoulderDashSnilku.Entities;

namespace BoulderDashSnilku.World {
    /// <summary>
    /// Groups all object created while loading one level.
    /// </summary>
    public class LoadedLevel {
        public GameWorld World { get; }
        public Player Player { get; }
        public LevelState LevelState { get; }
        public EntityManager EntityManager { get; }

        public LoadedLevel(GameWorld world, Player player,
            LevelState levelState, EntityManager entityManager) {
            World = world;
            Player = player;
            LevelState = levelState;
            EntityManager = entityManager;
        }
    }
}
