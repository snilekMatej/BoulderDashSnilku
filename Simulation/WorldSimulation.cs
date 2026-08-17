using System.Linq;
using BoulderDashSnilku.World;
using BoulderDashSnilku.Entities;
using Microsoft.Xna.Framework;

namespace BoulderDashSnilku.Simulation {
    /// <summary>
    /// Advances all time-based world mechanics at fixed rate.
    /// Updates in order falling objects first then enemies.
    /// </summary>
    public class WorldSimulation {
        private const double StepTime = 0.2;
        private const int EnemyMoveDelay = 3;
        private double _accumulator;
        private readonly BoulderLogic boulderLogic = new BoulderLogic();
        private readonly GemLogic gemLogic = new GemLogic();
        private readonly EnemyLogic enemyLogic = new EnemyLogic();
        private readonly ExplosionLogic explosionLogic = new ExplosionLogic();
        private bool[,] fallingObjects;
        public void Update(GameWorld world, EntityManager entityManager, GameTime gameTime) {
            _accumulator += gameTime.ElapsedGameTime.TotalSeconds;
            if (fallingObjects == null)
                fallingObjects = new bool[world.Width, world.Height];
            if (_accumulator >= StepTime) {
                _accumulator -= StepTime;
                bool[,] nextFallingObjects = new bool[world.Width, world.Height];
                bool[,] processedObjects = new bool[world.Width, world.Height];
                for (int y = world.Height - 2; y >= 0; y--) {
                    for (int x = 0; x < world.Width; x++) {
                        if (!processedObjects[x, y]) {
                            Tile tile = world.Grid[x, y];
                            if (world.Grid[x, y] == Tile.Boulder)
                                boulderLogic.Update(world, entityManager, explosionLogic,
                                    x, y, fallingObjects[x, y], nextFallingObjects,
                                    processedObjects);
                            else if (world.Grid[x, y] == Tile.Gem)
                                gemLogic.Update(world, entityManager, explosionLogic,
                                    x, y, fallingObjects[x, y], nextFallingObjects,
                                    processedObjects);
                        }
                    }
                }
                foreach (Enemy enemy in entityManager.GetEntities<Enemy>().ToList()) {
                    if (enemy.IsAlive) {
                        enemy.MoveTimer++;
                        if (enemy.MoveTimer >= EnemyMoveDelay) {
                            enemy.MoveTimer = 0;
                            enemyLogic.Update(enemy, world, entityManager, explosionLogic);
                        }
                    }
                }
                fallingObjects = nextFallingObjects;
            }
        }

        /// <summary>
        /// Crear timing and falling state information when level is loaded | reastarted.
        /// </summary>
        public void Reset() {
            _accumulator = 0;
            fallingObjects = null;
        }
    }
}
