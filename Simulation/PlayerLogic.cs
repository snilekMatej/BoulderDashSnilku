using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BoulderDashSnilku.Entities;
using BoulderDashSnilku.Input;
using BoulderDashSnilku.World;

namespace BoulderDashSnilku.Simulation
{
    public class PlayerLogic
    {
        public void Update(Player player, GameWorld world, EntityManager entities, MoveDirection direction)
        {
            int targetX = player.x;
            int targetY = player.y;

            switch (direction)
            {
                case MoveDirection.Left:
                    targetX--;
                    break;
                case MoveDirection.Right:
                    targetX++;
                    break;
                case MoveDirection.Up:
                    targetY--;
                    break;
                case MoveDirection.Down:
                    targetY++;
                    break;
                case MoveDirection.None:
                default:
                    return;
            }
            // Safety to not let player move off screen.
            if ((targetX < 0 || targetX >= world.Width) || (targetY < 0 || targetY >= world.Height))
            {
                return;
            }
            Tile targetTile = world.Grid[targetX, targetY];
            // colisions:
            if (targetTile == Tile.Wall || targetTile == Tile.Border)
            {
                return;
            }
            if (targetTile == Tile.Boulder)
            {
                if (!TryPushBoulder(world, entities, direction, targetX, targetY))
                {
                    return;
                }
            }
            if (targetTile == Tile.Dirt)
            {
                world.Grid[targetX, targetY] = Tile.Empty;
            }
            if (targetTile == Tile.Gem)
            {
                world.Grid[targetX, targetY] = Tile.Empty;
                // future: Score++;
            }

            player.MoveTo(targetX, targetY);
        }

        private bool TryPushBoulder(GameWorld world, EntityManager entities, MoveDirection direction, int boulderX, int boulderY)
        {
            int destinationX = boulderX;
            int destinationY = boulderY;

            switch (direction)
            {
                case MoveDirection.Left:
                    destinationX--;
                    break;
                case MoveDirection.Right:
                    destinationX++;
                    break;
                default:
                    return false;
            }
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
