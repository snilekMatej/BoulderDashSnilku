using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BoulderDashSnilku.Entities;
using BoulderDashSnilku.Input;
using BoulderDashSnilku.Library;
using BoulderDashSnilku.World;

namespace BoulderDashSnilku.Simulation
{
    public class ButterflyLogic
    {
        private readonly GameWorld world;
        private readonly EntityManager entityManager;

        public void Update(Butterfly butterfly, GameWorld world, EntityManager entityManager, ExplosionLogic explosionLogic)
        {
            Direction right = butterfly.Direction.TurnRight();

            if (CanMove(butterfly, right, world, entityManager))
            {
                butterfly.Direction = right;
                Move(butterfly, right, world, entityManager, explosionLogic);
            }
            else if (CanMove(butterfly, butterfly.Direction, world, entityManager))
            {
                Move(butterfly, butterfly.Direction, world, entityManager, explosionLogic);
            }
            else
            {
                butterfly.Direction = butterfly.Direction.TurnLeft();
            }
        }
        private bool CanMove(Butterfly butterfly, Direction direction, GameWorld world, EntityManager entityManager)
        {
            (int offsetX, int offsetY) = direction.GetOffset();

            int targetX = butterfly.x + offsetX;
            int targetY = butterfly.y + offsetY;

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
        private void Move(Butterfly butterfly, Direction direction, GameWorld world, EntityManager entityManager, ExplosionLogic explosionLogic)
        {
            (int offsetX, int offsetY) = direction.GetOffset();
            int targetX = butterfly.x + offsetX;
            int targetY = butterfly.y + offsetY;

            Entity? targetEntity = entityManager.GetEntityAt(targetX, targetY);

            if (targetEntity is Player player)
            {
                player.Kill(world, entityManager, explosionLogic);
                return;
            }
            butterfly.MoveTo(targetX, targetY);
        }

    }
}
