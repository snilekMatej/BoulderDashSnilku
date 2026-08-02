using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BoulderDashSnilku.Core;
using BoulderDashSnilku.Entities;
using BoulderDashSnilku.Input;
using BoulderDashSnilku.World;
using BoulderDashSnilku.Library;

namespace BoulderDashSnilku.Simulation
{
    public class PlayerLogic
    {
        public void Update(Player player, GameWorld world, EntityManager entities, LevelState levelState, GameSession gameSession, MoveDirection direction, ExplosionLogic explosionLogic)
        {
            if (direction == MoveDirection.None)
            {
                return;
            }
            (int offsetX, int offsetY) = direction.GetOffset();

            int targetX = player.x + offsetX;
            int targetY = player.y + offsetY;

            // Safety to not let player move off screen.
            if (!world.IsInBounds(targetX, targetY))
            {
                return;
            }
            // entities
            Entity? targetEntity = entities.GetEntityAt(targetX, targetY);

            if (targetEntity is Enemy)
            {
                player.Kill(world, entities, explosionLogic);
                return;
            }

            Tile targetTile = world.Grid[targetX, targetY];
            // colisions:
            switch (targetTile)
            {
                case Tile.Wall or Tile.Border:
                    return;
                case Tile.Boulder:
                    if (!TryPushBoulder(world, entities, direction, targetX, targetY))
                    {
                        return;
                    }
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

            player.MoveTo(targetX, targetY);
        }

        private bool TryPushBoulder(GameWorld world, EntityManager entities, MoveDirection direction, int boulderX, int boulderY)
        {
            if (direction is not (MoveDirection.Left or MoveDirection.Right))
            {
                return false;
            }

            (int offsetX, int offsetY) = direction.GetOffset();

            int destinationX = boulderX + offsetX;
            int destinationY = boulderY + offsetY;

            if (world.Grid[destinationX, destinationY] != Tile.Empty || entities.HasEntityAt(destinationX, destinationY))
            {
                return false;
            }
            world.Grid[destinationX, destinationY] = Tile.Boulder;
            world.Grid[boulderX, boulderY] = Tile.Empty;

            return true;
        }
    }
}
