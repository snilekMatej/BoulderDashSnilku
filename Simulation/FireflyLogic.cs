using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using BoulderDashSnilku.Entities;
using BoulderDashSnilku.Input;
using BoulderDashSnilku.Library;
using BoulderDashSnilku.World;

namespace BoulderDashSnilku.Simulation
{
    public class FireflyLogic
    {
        private readonly GameWorld world;
        private readonly EntityManager entityManager;

        public void Update(Firefly firefly, GameWorld world, EntityManager entityManager)
        {
            Direction left = firefly.Direction.TurnLeft();

            if (CanMove(firefly, left, world, entityManager))
            {
                firefly.Direction = left;
                Move(firefly, left, entityManager);
            }
            else if (CanMove(firefly, firefly.Direction, world, entityManager))
            {
                Move(firefly, firefly.Direction, entityManager);
            }
            else
            {
                firefly.Direction = firefly.Direction.TurnRight();
            }
        }
        private bool CanMove(Firefly firefly, Direction direction, GameWorld world, EntityManager entityManager)
        {
            (int offsetX, int offsetY) = direction.GetOffset();

            int targetX = firefly.x + offsetX;
            int targetY = firefly.y + offsetY;

            if (targetX < 0 || targetX >= world.Width || targetY < 0 || targetY >= world.Height)
            {
                return false;
            }
            if (world.Grid[targetX, targetY] != Tile.Empty)
            {
                return false;
            }
            if (entityManager.HasEntityAt(targetX, targetY))
            {
                if (entityManager.GetEntityAt(targetX, targetY) is Player)
                {
                    return true;
                }
                return false;
            }
            return true;
        }
        private void Move(Firefly firefly, Direction direction, EntityManager entityManager)
        {
            (int offsetX, int offsetY) = direction.GetOffset();
            int targetX = firefly.x + offsetX;
            int targetY = firefly.y + offsetY;

            Entity? targetEntity = entityManager.GetEntityAt(targetX, targetY);

            if (targetEntity is Player player)
            {
                player.Kill();
                entityManager.Remove(player);
                return;
            }
            firefly.MoveTo(targetX, targetY);
        }

    }
}
