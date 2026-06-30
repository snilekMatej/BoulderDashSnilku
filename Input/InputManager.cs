using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private KeyboardState _previous;
        private KeyboardState _current;

        private double _holdTimer = 0;
        private const double Holdinterval = 0.1; // 100 millisecnds
        private MoveDirection _lastDirection = MoveDirection.None;

        public void Update(GameTime gameTime)
        {
            _previous = _current;
            _current = Keyboard.GetState();

            _holdTimer += gameTime.ElapsedGameTime.TotalSeconds;
        }

        public MoveDirection GetMoveDirection()
        {
            // IMPORTANT: edge detection (one press = one move)
            if (IsPressed(Keys.Up)) return MoveDirection.Up;
            if (IsPressed(Keys.Down)) return MoveDirection.Down;
            if (IsPressed(Keys.Left)) return MoveDirection.Left;
            if (IsPressed(Keys.Right)) return MoveDirection.Right;

            return MoveDirection.None;
        }

        private bool IsPressed(Keys key)
        {
            return _current.IsKeyDown(key) && !_previous.IsKeyDown(key);
        }
    }
}
