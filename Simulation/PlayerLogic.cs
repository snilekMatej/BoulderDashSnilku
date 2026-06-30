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
        public void Update(Player player, GameWorld world, MoveDirection direction)
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
            player.MoveTo(targetX, targetY);
        }
    }
}
