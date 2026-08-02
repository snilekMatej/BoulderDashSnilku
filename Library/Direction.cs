using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoulderDashSnilku.Input;

namespace BoulderDashSnilku.Library
{
    public enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }
    public static class DirectionExtentions
    {
        public static Direction TurnLeft(this Direction direction)
        {
            return direction switch
            {
                Direction.Up => Direction.Left,
                Direction.Left => Direction.Down,
                Direction.Down => Direction.Right,
                Direction.Right => Direction.Up,
                
                _ => direction
            };
        }
        public static Direction TurnRight(this Direction direction)
        {
            return direction switch
            {
                Direction.Up => Direction.Right,
                Direction.Right => Direction.Down,
                Direction.Down => Direction.Left,
                Direction.Left => Direction.Up,

                _ => direction
            };
        }
        public static (int x, int y) GetOffset(this Direction direction)
        {
            return direction switch
            {
                Direction.Up => (0, -1),
                Direction.Right => (1, 0),
                Direction.Down => (0, 1),
                Direction.Left => (-1, 0),

                _ => (0, 0)
            };
        }

        public static (int x, int y) GetOffset(this MoveDirection direction)
        {
            return direction switch
            {
                MoveDirection.Up => (0, -1),
                MoveDirection.Right => (1, 0),
                MoveDirection.Down => (0, 1),
                MoveDirection.Left => (-1, 0),
                _ => (0, 0)
            };
        }
    }
}
