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
        private MoveDirection _heldDirection = MoveDirection.None;

        public void Update(GameTime gameTime)
        {
            _previous = _current;
            _current = Keyboard.GetState();

            _holdTimer += gameTime.ElapsedGameTime.TotalSeconds;
        }

        public MoveDirection GetMoveDirection()
        {
            MoveDirection Direction = MoveDirection.None;

            if (_current.IsKeyDown(Keys.Up))
            {
                Direction = MoveDirection.Up;
            }
            if (_current.IsKeyDown(Keys.Down))
            {
                Direction = MoveDirection.Down;
            }
            if (_current.IsKeyDown(Keys.Left))
            {
                Direction = MoveDirection.Left;
            }
            if (_current.IsKeyDown(Keys.Right))
            {
                Direction = MoveDirection.Right;
            }

            if (Direction == MoveDirection.None)
            {
                _heldDirection = Direction;
                _holdTimer = 0;
                return MoveDirection.None;
            }
            if (Direction != _heldDirection)
            {
                _heldDirection = Direction;
                _holdTimer = 0;
                return Direction;
            }
            if (_holdTimer >= Holdinterval)
            {
                _holdTimer = 0;
                return _heldDirection;
            }
            return MoveDirection.None;
        }

        private bool IsPressed(Keys key)
        {
            return _current.IsKeyDown(key) && !_previous.IsKeyDown(key);
        }
    }
}
