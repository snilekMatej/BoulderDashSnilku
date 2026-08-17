using BoulderDashSnilku.Entities;
using BoulderDashSnilku.Library;
using BoulderDashSnilku.World;

namespace BoulderDashSnilku.Simulation {
    /// <summary>
    /// Controls movement and player colisions with all enemies.
    /// Enemies share similar movement logic.
    /// </summary>
    class EnemyLogic {
        /// <summary>
        /// Updates one enemy based on its preffered movement directions.
        /// </summary>
        public void Update(Enemy enemy, GameWorld world,
            EntityManager entityManager, ExplosionLogic explosionLogic) {
            Direction preferredDirection = enemy.GetPreferredDirection();
            if (CanMove(enemy, preferredDirection, world, entityManager)) {
                enemy.Direction = preferredDirection;
                Move(enemy, preferredDirection, world, entityManager, explosionLogic);
            } else if (CanMove(enemy, enemy.Direction, world, entityManager)) {
                Move(enemy, enemy.Direction, world, entityManager, explosionLogic);
            } else enemy.Direction = enemy.GetBlockedTurn();
        }
        
        /// <summary>
        /// Check if enemy can enter neighbouring tile in specified direction.
        /// Empty tile and Player are concidered passable
        /// </summary>
        /// <returns>True -> Tile is passable.</returns>
        private bool CanMove(Enemy enemy, Direction direction,
            GameWorld world, EntityManager entityManager) {
            (int offsetX, int offsetY) = direction.GetOffset();
            int targetX = enemy.x + offsetX;
            int targetY = enemy.y + offsetY;
            bool canMove = world.IsInBounds(targetX, targetY);
            if (canMove) canMove = world.Grid[targetX, targetY] == Tile.Empty;
            if (canMove && entityManager.HasEntityAt(targetX, targetY))
                canMove = entityManager.GetEntityAt(targetX, targetY) is Player;
            return canMove;
        }

        /// <summary>
        /// Move enemy one tile in given direction.
        /// If Player is in its way Player is killed.
        /// </summary>
        private void Move(Enemy enemy, Direction direction, GameWorld world,
            EntityManager entityManager, ExplosionLogic explosionLogic) {
            (int offsetX, int offsetY) = direction.GetOffset();
            int targetX = enemy.x + offsetX;
            int targetY = enemy.y + offsetY;
            Entity? targetEntity = entityManager.GetEntityAt(targetX, targetY);
            if (targetEntity is Player player)
                player.Kill(world, entityManager, explosionLogic);
            else enemy.MoveTo(targetX, targetY);
        }
    }
}
