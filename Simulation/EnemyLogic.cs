using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BoulderDashSnilku.Entities;
using BoulderDashSnilku.Library;
using BoulderDashSnilku.World;

namespace BoulderDashSnilku.Simulation
{
    class EnemyLogic
    {
        public void Update(Enemy enemy, GameWorld world, EntityManager entityManager, ExplosionLogic explosionLogic)
        {
            Direction preferedDirection = enemy.GetPreferredDirection();

            if (CanMove(enemy, preferedDirection, world, entityManager))
            {
                enemy.Direction = preferedDirection;
                Move(enemy, preferedDirection, world, entityManager, explosionLogic);
            }
            else if (CanMove(enemy, enemy.Direction, world, entityManager))
            {
                Move(enemy, enemy.Direction, world, entityManager, explosionLogic);
            }
            else
            {
                enemy.Direction = enemy.GetBlockedTurn();
            }
        }
        private bool CanMove(Enemy enemy, Direction direction, GameWorld world, EntityManager entityManager)
        {
            (int offsetX, int offsetY) = direction.GetOffset();

            int targetX = enemy.x + offsetX;
            int targetY = enemy.y + offsetY;

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
                return entityManager.GetEntityAt(targetX, targetY) is Player;
            }
            return true;
        }
        private void Move(Enemy enemy, Direction direction, GameWorld world, EntityManager entityManager, ExplosionLogic explosionLogic)
        {
            (int offsetX, int offsetY) = direction.GetOffset();
            int targetX = enemy.x + offsetX;
            int targetY = enemy.y + offsetY;

            Entity? targetEntity = entityManager.GetEntityAt(targetX, targetY);

            if (targetEntity is Player player)
            {
                player.Kill(world, entityManager, explosionLogic);
                return;
            }
            enemy.MoveTo(targetX, targetY);
        }
    }
}
