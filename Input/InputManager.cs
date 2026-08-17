using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BoulderDashSnilku.Input {
    /// <summary>
    /// Possible movement directions requested by player.
    /// `None` represents frame with none occured movement.
    /// </summary>
    public enum MoveDirection {
        None,
        Left,
        Right,
        Up,
        Down
    }

    /// <summary>
    /// Reads keyboard input and converts held arrow keys to timed movement.
    /// </summary>
    class InputManager {
        private const double HoldInterval = 0.125; // 125 milliseconds
        private KeyboardState previousState;
        private KeyboardState currentState;
        private double holdTimer = 0;
        private MoveDirection heldDirection = MoveDirection.None;

        /// <summary>
        /// Stores current keyboard state.
        /// Advances the key-held timer.
        /// Must be called once per frame before reading input.
        /// </summary>
        /// <param name="gameTime">Timing information for current frame.</param>
        public void Update(GameTime gameTime) {
            previousState = currentState;
            currentState = Keyboard.GetState();
            holdTimer += gameTime.ElapsedGameTime.TotalSeconds;
        }

        /// <summary>
        /// Get player movement and controll how fast he moves.
        /// Pressed direction is imediet and held direction repeats after HoldInterval.
        /// </summary>
        /// <returns>Direction in which the player moves to | None.</returns>
        public MoveDirection GetMoveDirection() {
            MoveDirection direction = GetHeldDirection();
            MoveDirection moveDirection = MoveDirection.None;
            if (direction == MoveDirection.None) ResetHold();
            else if (direction != heldDirection) {
                heldDirection = direction;
                moveDirection = direction;
                holdTimer = 0;
            }
            else if (holdTimer >= HoldInterval) {
                holdTimer = 0;
                moveDirection = heldDirection;
            }
            return moveDirection;
        }

        /// <summary>
        /// Determines which movement key is being held.
        /// Directions are checked in fixed order when multiple are held.
        /// </summary>
        /// <returns>Held direction | None</returns>
        private MoveDirection GetHeldDirection() {
            MoveDirection direction = MoveDirection.None;
            if (currentState.IsKeyDown(Keys.Up)) direction = MoveDirection.Up;
            else if (currentState.IsKeyDown(Keys.Down)) direction = MoveDirection.Down;
            else if (currentState.IsKeyDown(Keys.Left)) direction = MoveDirection.Left;
            else if (currentState.IsKeyDown(Keys.Right)) direction = MoveDirection.Right;
            return direction;
        }

        /// <summary>
        /// Clears currently held movement direction and its repeat timer.
        /// 
        /// </summary>
        private void ResetHold() {
            heldDirection = MoveDirection.None;
            holdTimer = 0;
        }

        /// <summary>
        /// Checks if key became pressed in current frame.
        /// </summary>
        /// <param name="key">Keyboard key to check.</param>
        /// <returns>True -> WasKeyDown (this frame && not previous frame).</returns>
        public bool IsPressed(Keys key) {
            return currentState.IsKeyDown(key) && !previousState.IsKeyDown(key);
        }
    }
}
