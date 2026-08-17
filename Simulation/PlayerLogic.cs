using BoulderDashSnilku.Core;
using BoulderDashSnilku.Entities;
using BoulderDashSnilku.Input;
using BoulderDashSnilku.World;
using BoulderDashSnilku.Library;

namespace BoulderDashSnilku.Simulation {
    /// <summary>
    /// Handles player movement and interacton iwth tiles and entities.
    /// Includes digging, gem collecting, pushing boulders, entering exits and enemy colision.
    /// </summary>
    public class PlayerLogic {

        /// <summary>
        /// Attempt to move player one tile in given direction.
        /// All interactions are handled before position is changed.
        /// </summary>
        public void Update(Player player, GameWorld world, EntityManager entityManager,
            LevelState levelState, GameSession gameSession, MoveDirection direction,
            ExplosionLogic explosionLogic) {
            bool canMove = direction != MoveDirection.None;
            int targetX = player.x;
            int targetY = player.y;
            if (canMove) {
                (int offsetX, int offsetY) = direction.GetOffset();
                targetX = player.x + offsetX;
                targetY = player.y + offsetY;
                canMove = world.IsInBounds(targetX, targetY);
            } if (canMove) {
                Entity? targetEntity = entityManager.GetEntityAt(targetX, targetY);
                if (targetEntity is Enemy) {
                    player.Kill(world, entityManager, explosionLogic);
                    canMove = false;
                }
            } if (canMove)
            {
                Tile targetTile = world.Grid[targetX, targetY];
                switch (targetTile) {
                    case Tile.Wall or Tile.Border:
                        canMove = false;
                        break;
                    case Tile.Boulder:
                        canMove = TryPushBoulder(
                            world, entityManager, direction, targetX, targetY);
                        break;
                    case Tile.Dirt:
                        world.Grid[targetX, targetY] = Tile.Empty;
                        break;
                    case Tile.Gem:
                        world.Grid[targetX, targetY] = Tile.Empty;
                        levelState.CollectGem(world);
                        gameSession.Score += levelState.GemValue;
                        break;
                    case Tile.Exit:
                        levelState.CompleteLevel();
                        break;
                }
            }
            if (canMove) player.MoveTo(targetX, targetY);
        }

        /// <summary>
        /// Attempt to push a boulder to the side to empty tile.
        /// </summary>
        /// <returns>True -> boulder was successfully moved.</returns>
        private bool TryPushBoulder(GameWorld world, EntityManager entities,
            MoveDirection direction, int boulderX, int boulderY) {
            bool canPush = direction is  MoveDirection.Left or MoveDirection.Right;
            int destinationX = boulderX;
            int destinationY = boulderY;
            if (canPush) {
                (int offsetX, int offsetY) = direction.GetOffset();
                destinationX = boulderX + offsetX;
                destinationY = boulderY + offsetY;
                canPush = world.Grid[destinationX, destinationY] == Tile.Empty &&
                    !entities.HasEntityAt(destinationX, destinationY);
            } if (canPush) {
                world.Grid[destinationX, destinationY] = Tile.Boulder;
                world.Grid[boulderX, boulderY] = Tile.Empty;
            }
            return canPush;
        }
    }
}
