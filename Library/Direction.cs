using BoulderDashSnilku.Input;

namespace BoulderDashSnilku.Library {
    /// <summary>
    /// Represents one of four directions used by moving entities.
    /// </summary>
    public enum Direction {
        Up,
        Right,
        Down,
        Left
    }

    /// <summary>
    /// Provides operations for rotating directions and converting direction into coordinates.
    /// Support for entity `Direction` and player `MoveDirection`.
    /// </summary>
    public static class DirectionExtentions {
        /// <summary>
        /// Rotate direction 90 deg counterClockwise.
        /// </summary>
        /// <param name="direction">Direction to rotate.</param>
        /// <returns>Direction to the left of given direction.</returns>
        public static Direction TurnLeft(this Direction direction) {
            return direction switch {
                Direction.Up => Direction.Left,
                Direction.Left => Direction.Down,
                Direction.Down => Direction.Right,
                Direction.Right => Direction.Up,
                _ => direction
            };
        }

        /// <summary>
        /// Rotate direction 90 deg clockwise.
        /// </summary>
        /// <param name="direction">Direction to rotate.</param>
        /// <returns>Direction to the right of given direction.</returns>
        public static Direction TurnRight(this Direction direction) {
            return direction switch {
                Direction.Up => Direction.Right,
                Direction.Right => Direction.Down,
                Direction.Down => Direction.Left,
                Direction.Left => Direction.Up,
                _ => direction
            };
        }

        /// <summary>
        /// Convert entity direction to its destination coordinates.
        /// </summary>
        /// <param name="direction">Direction to convert.</param>
        /// <returns>X and Y offsets for one tile movement.</returns>
        public static (int x, int y) GetOffset(this Direction direction) {
            return direction switch {
                Direction.Up => (0, -1),
                Direction.Right => (1, 0),
                Direction.Down => (0, 1),
                Direction.Left => (-1, 0),
                _ => (0, 0)
            };
        }

        /// <summary>
        /// Convert player direction to its destination coordinates.
        /// </summary>
        /// <param name="direction">MoveDirection to convert.</param>
        /// <returns>X and Y offset for one tile movement.</returns>
        public static (int x, int y) GetOffset(this MoveDirection direction) {
            return direction switch {
                MoveDirection.Up => (0, -1),
                MoveDirection.Right => (1, 0),
                MoveDirection.Down => (0, 1),
                MoveDirection.Left => (-1, 0),
                _ => (0, 0)
            };
        }
    }
}
