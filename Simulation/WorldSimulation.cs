using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoulderDashSnilku.World;
using BoulderDashSnilku.Entities;
using Microsoft.Xna.Framework;

namespace BoulderDashSnilku.Simulation
{
    public class WorldSimulation
    {
        private double _accumulator = 0;
        private const double StepTime = 0.125;
        private const double EnemyStepTime = 0.375;

        private BoulderLogic boulderLogic = new BoulderLogic();
        private GemLogic gemLogic = new GemLogic();
        private EnemyLogic enemyLogic = new EnemyLogic();
        private ExplosionLogic explosionLogic = new ExplosionLogic();

        private bool[,] fallingObjects;
        public void Update(GameWorld world, EntityManager entityManager, GameTime gameTime)
        {
            _accumulator += gameTime.ElapsedGameTime.TotalSeconds;

            if (fallingObjects == null)
            {
                fallingObjects = new bool[world.Width, world.Height];
            }
            bool[,] nextFallingObjects = new bool[world.Width, world.Height];

            if (_accumulator < StepTime)
            {
                return;
            }
            _accumulator = 0;

            for (int y = world.Height - 2;  y >= 0; y--)
            {
                for (int x = 0; x < world.Width; x++)
                {
                    if (world.Grid[x, y] == Tile.Boulder)
                    {
                        boulderLogic.Update(world, entityManager, explosionLogic, x, y, fallingObjects[x, y], nextFallingObjects);
                    } 
                    if (world.Grid[x, y] == Tile.Gem)
                    {
                        gemLogic.Update(world, entityManager, explosionLogic, x, y, fallingObjects[x, y], nextFallingObjects);
                    }
                }
            }
            
            foreach (Enemy enemy in entityManager.GetEntities<Enemy>().ToList())
            {
                if (enemy.IsAlive)
                {
                    enemy.MoveTimer++;
                    if (enemy.MoveTimer >= 3)
                    {
                        enemy.MoveTimer = 0;
                        enemyLogic.Update(enemy, world, entityManager, explosionLogic);
                    }
                }
            }
            fallingObjects = nextFallingObjects;
        }
    }
}
