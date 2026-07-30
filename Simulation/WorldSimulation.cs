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
        private FireflyLogic fireflyLogic = new FireflyLogic();

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
                        boulderLogic.Update(world, entityManager, x, y, fallingObjects[x, y], nextFallingObjects);
                    } 
                    if (world.Grid[x, y] == Tile.Gem)
                    {
                        gemLogic.Update(world, entityManager, x, y, fallingObjects[x, y], nextFallingObjects);
                    }
                }
            }
            
            foreach (Firefly firefly in entityManager.GetEntities<Firefly>().ToList())
            {
                if (firefly.IsAlive)
                {
                    firefly.MoveTimer++;
                    if (firefly.MoveTimer >= 3)
                    {
                        firefly.MoveTimer = 0;
                        fireflyLogic.Update(firefly, world, entityManager);
                    }
                }
            }
            fallingObjects = nextFallingObjects;
        }
    }
}
