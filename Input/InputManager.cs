using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoulderDashSnilku.Library;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BoulderDashSnilku.Input
{
    public enum MoveDirection
    {
        None,
        Left,
        Right,
        Up,
        Down
    }
    class InputManager
    {
        private const double HoldInterval = 0.1; // 100 millisecnds

        private KeyboardState previousState;
        private KeyboardState currentState;

        private double holdTimer = 0;
        private MoveDirection heldDirection = MoveDirection.None;

        public void Update(GameTime gameTime)
        {
            previousState = currentState;
            currentState = Keyboard.GetState();

            holdTimer += gameTime.ElapsedGameTime.TotalSeconds;
        }

        public MoveDirection GetMoveDirection()
        {
            MoveDirection direction = GetHeldDirection();

            if (direction != MoveDirection.None)
            {
                ResetHold();
                return direction;
            }
            if (direction != heldDirection)
            {
                heldDirection = direction;
                holdTimer = 0;

                return direction;
            }
            if (holdTimer >= 0)
            {
                holdTimer = 0;
                return heldDirection;
            }
            return MoveDirection.None;
        }

        private MoveDirection GetHeldDirection()
        {
            if (currentState.IsKeyDown(Keys.Up))
            {
                return MoveDirection.Up;
            }
            if (currentState.IsKeyDown(Keys.Down))
            {
                return MoveDirection.Down;
            }
            if (currentState.IsKeyDown(Keys.Left))
            {
                return MoveDirection.Left;
            }
            if (currentState.IsKeyDown(Keys.Right))
            {
                return MoveDirection.Right;
            }
            return MoveDirection.None;
        }

        private void ResetHold()
        {
            heldDirection = MoveDirection.None;
            holdTimer = 0;
        }
        public bool IsPressed(Keys key)
        {
            return currentState.IsKeyDown(key) && !previousState.IsKeyDown(key);
        }
    }
}
